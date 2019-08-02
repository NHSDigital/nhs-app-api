/* eslint-disable */

/*jshint esversion: 6 */
/*global fetch, btoa */
import $q from 'q';
import axios from 'axios'
import isEmpty from 'lodash/fp/isEmpty'
import get from 'lodash/fp/get'
import {
	resolveApiClient,
} from '../middleware/urlResolution'
import { v4 as uuid } from 'uuid'

const getWebVersion = get('store.state.appVersion.webVersion');
const getNativeVersion = get('store.state.appVersion.nativeVersion');
const getPlatform = get('store.state.device.source');

/**
 * This is the API for the NHS App.
 * @class NHSOnlineApi
 * @param {(string|object)} [domainOrOptions] - The project domain or options object.
 *                                              If object, see the object's optional properties.
 * @param {string} [domainOrOptions.domain] - The project domain
 * @param {object} [domainOrOptions.token] - auth token - object with value property and optional
 *                                                        headerOrQueryName and isQuery properties
 */
class CDSApi {
	constructor(options) {
		this.store = options.store;
		this.res = options.res;
		this.req = options.req;
		this.cookies = options.cookies;
		this.__domain = options.domain;
	}

	get domain() {
		const {
			app: {
				$env
			}
		} = this.store;
		const domain = this.req ?
			$env.API_HOST_SERVER :
			resolveApiClient({
				host: this.req,
				env: $env
			});

		return domain;
	}

	serializeQueryParams(parameters) {
		let str = [];
		for (let p in parameters) {
			if (parameters.hasOwnProperty(p)) {
				str.push(encodeURIComponent(p) + '=' + encodeURIComponent(parameters[p]));
			}
		}
		return str.join('&');
	}

	mergeQueryParams(parameters, queryParameters) {
		if (parameters.$queryParameters) {
			Object.keys(parameters.$queryParameters)
				.forEach(function(parameterName) {
					let parameter = parameters.$queryParameters[parameterName];
					queryParameters[parameterName] = parameter;
				});
		}
		return queryParameters;
	}

	/**
	 * HTTP Request
	 * @method
	 * @name NHSOnlineApi#request
	 * @param {string} method - http method
	 * @param {string} url - url to do request
	 * @param {object} parameters
	 * @param {object} body - body parameters / object
	 * @param {object} headers - header parameters
	 * @param {object} queryParameters - querystring parameters
	 * @param {object} form - form data object
	 * @param {object} deferred - promise object
	 * @param {object} ignoreError - boolean to control dispatching ApiError message
	 */
	request({
		method,
		url,
		parameters,
		body,
		headers,
		queryParameters,
		form,
		deferred,
		ignoreError,
	}) {
		const queryParams = queryParameters && Object.keys(queryParameters).length ? this.serializeQueryParams(queryParameters) : null;
		const urlWithParams = url + (queryParams ? '?' + queryParams : '');

		if (body && !Object.keys(body).length) {
			body = undefined;
		}

		const CancelToken = axios.CancelToken;
		let cancel;

		this.store.dispatch('http/isLoading');

		const csrfToken = get('csrfToken')(parameters) || get('store.state.session.csrfToken')(this);
		if (csrfToken) {
			headers['X-CSRF-TOKEN'] = csrfToken;
		}

		const cookie = get('cookie')(parameters) || this.cookie;
		if (cookie) {
			headers['Cookie'] = cookie;
    }
    
    let nhsoRequestId;

    if (process.server) {
      nhsoRequestId = this.res.locals.nhsoRequestId;
    } else {
      nhsoRequestId = uuid();
    }
    
    headers['NHSO-Request-ID'] = nhsoRequestId;

		const webVersion = getWebVersion(this);
		const nativeVersion = getNativeVersion(this);
		const platform = getPlatform(this);

		if (webVersion) {
			headers['NHSO-Web-Version-Tag'] = webVersion + ' (commit:' + this.store.app.$env.COMMIT_ID + ')';
		}
		if (nativeVersion) {
			if (platform !== 'web') {
				let fullNativeVersion = platform;
				fullNativeVersion += ' ';
				fullNativeVersion += nativeVersion;

				headers['NHSO-Native-Version-Tag'] = fullNativeVersion;
			} else {
				headers['NHSO-Native-Version-Tag'] = nativeVersion;
			}
		}

		const resolve = ({
			store,
			deferred,
			result,
			cancel
		}) => {
			deferred.resolve(result)
			if (store && store.dispatch) store.dispatch('http/addCancelRequestHandler', cancel);
		};

		const reject = ({
			store,
			deferred,
			error,
			cancel
		}) => {
			deferred.reject(error)
			if (store && store.dispatch) store.dispatch('http/addCancelRequestHandler', cancel);
		};

		const requestStartTime = new Date();

		let consola = {};

		if (process.server) {
			consola = require('consola');
			consola.info(`Begin request: ${method} ${urlWithParams}`);
		}

		axios({
			cancelToken: new CancelToken(function executor(c) {
				// An executor function receives a cancel function as a parameter
				cancel = c;
			}),
			url: urlWithParams,
			method,
			headers,
			withCredentials: true,
			crossDomain: true,
			data: JSON.stringify(body)
		}).then((response) => {
			if (process.server) {
				this.res.setHeader('Cache-Control', `${response.headers['cache-control']}`);
				this.res.setHeader('Pragma', `${response.headers['pragma']}`);

				if (response) {
					const requestDurationMilliseconds = new Date().getTime() - requestStartTime.getTime();
					consola.info(`End request: ${method} ${urlWithParams}, response: ${response.status}, duration: ${requestDurationMilliseconds}ms`);
				}
				if (response.headers['set-cookie']) {
					this.res.setHeader('Set-Cookie', `${response.headers['set-cookie']}; SameSite=Lax`);
				}
			}

			return response.data
		}).then((body) => {
			this.store.dispatch('http/loadingCompleted');
			resolve({
				deferred,
				result: body,
				store: this.store
			});
		}).catch((error) => {
			if (process.server) {
				const requestDurationMilliseconds = new Date().getTime() - requestStartTime.getTime();

				if (error.response) {
					consola.error(new Error(`Error response for request: ${method} ${urlWithParams}, response: ${error.response.status}, duration: ${requestDurationMilliseconds}ms}`));
				} else if (error.request) {
					consola.error(new Error(`Error sending request: ${method} ${urlWithParams}, duration: ${requestDurationMilliseconds}ms`));
				} else {
					consola.error(new Error(`Error setting up the request: ${method} ${urlWithParams}, error: ${error.message}, duration: ${requestDurationMilliseconds}ms`));
				}
			}
			this.store.dispatch('http/loadingCompleted');
			if (!axios.isCancel(error)) {
				if (error.response !== undefined && 401 === error.response.status) {
					this.store.dispatch('auth/unauthorised');
					resolve({
						deferred,
						store: this.store
					});
				}

				if (!ignoreError) {
					this.store.dispatch('errors/addApiError', error);
				}

				if (process.server && !isEmpty(this.store.state.errors.apiErrors)) {
					resolve({
						deferred,
						store: this.store
					});
				} else {
					reject({
						deferred,
						error,
						store: this.store
					});
				}
			}

			reject({
				deferred,
				error
			});
		});
  }

  /**
	 * Evaluates the given Fhir Parameters resource and returns a Fhir GuidanceResponse 
	 * @method
	 * @name CDSApi#postFhirServiceDefinitionEvaluate
	 * @param {object} parameters - method options and parameters
	 * @param {object} parameters.parameters - the wrapping parameters resource to be evaluated
	 */
	postFhirServiceDefinitionEvaluate(parameters) {
		if (parameters === undefined) {
			parameters = {};
		}
		let ignoreError = parameters.ignoreError || false;
		let deferred = $q.defer();
		let domain = this.domain;
		let path = '/fhir/ServiceDefinition/{provider}/{id}/$evaluate';
		let body = {};
		let headers = {};
		let form = {};

		let queryParameters = {};

    headers['Content-Type'] = ['application/json+fhir'];

    if (parameters['serviceDefinition'] === undefined) {
      deferred.reject(new Error('Missing required parameter: serviceDefinitionId'));
      return deferred.promise;
    }

    if (parameters['provider'] === undefined) {
      deferred.reject(new Error('Missing required parameter: provider'));
      return deferred.promise;
    }

		if (parameters['parameters'] === undefined) {
			deferred.reject(new Error('Missing required parameter: parameters'));
			return deferred.promise;
		}
		
		if (parameters['addJavascriptDisabledHeader']) {
			headers['NHSO-Javascript-Disabled'] = 'true';
		}

    path = path.replace('{id}', parameters['serviceDefinition']);
    path = path.replace('{provider}', parameters['provider']);

    body = parameters['parameters'];

		queryParameters = this.mergeQueryParams(parameters, queryParameters);

		this.request({
			method: 'POST',
			url: domain + path,
			parameters,
			body,
			headers,
			queryParameters,
			form,
			deferred,
			ignoreError
		});

		return deferred.promise;
	}

	/**
	 * Gets the Service Definition resource for the given ID
	 * @method
	 * @name CDSApi#getFhirSer\viceDefinition
   * @param {string} parameters.provider - the provider of the online consultation journey
	 */
	getFhirServiceDefinition(parameters) {
		if (parameters === undefined) {
			parameters = {};
		}
		let ignoreError = parameters.ignoreError || false;
		let deferred = $q.defer();
		let domain = this.domain;
		let path = '/fhir/ServiceDefinition/{provider}/{id}';
		let body = {};
		let headers = {};
		let form = {};

		let queryParameters = {};

		headers['Accept'] = ['application/json+fhir'];

    if (parameters['serviceDefinition'] === undefined) {
      deferred.reject(new Error('Missing required parameter: serviceDefinitionId'));
      return deferred.promise;
    }

    if (parameters['provider'] === undefined) {
      deferred.reject(new Error('Missing required parameter: provider'));
      return deferred.promise;
    }

    path = path.replace('{provider}', parameters['provider']);
    path = path.replace('{id}', parameters['serviceDefinition']);

		queryParameters = this.mergeQueryParams(parameters, queryParameters);

		this.request({
			method: 'GET',
			url: domain + path,
			parameters,
			body,
			headers,
			queryParameters,
			form,
			deferred,
			ignoreError
		});

		return deferred.promise;
	}
}

export default CDSApi;
