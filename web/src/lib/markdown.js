/* eslint-disable no-param-reassign */
import MarkdownIt from 'markdown-it';
import isEmpty from 'lodash/fp/isEmpty';
import { isSameHostNameAndProtocol, stripHtml } from '@/lib/utils';
import { REDIRECT_PARAMETER } from '@/router/names';
import { INTERSTITIAL_REDIRECTOR_PATH } from '@/router/paths';

let markdownInternal;
let $store;

const overrideLinkForRedirector = (token, href, hrefIndex) => {
  if (!isEmpty($store)) {
    const matchedService = $store.getters['knownServices/matchOneByUrl'](href) || {};
    if (matchedService.requiresAssertedLoginIdentity) {
      token.attrs[hrefIndex][1] = `/${INTERSTITIAL_REDIRECTOR_PATH}?${REDIRECT_PARAMETER}=${encodeURIComponent(href)}`;
    }
  }
};

const assignTarget = (token, href) => {
  let targetValue = '_blank';
  if (href.startsWith('/') && !href.startsWith('//')) {
    targetValue = '_self';
  } else {
    targetValue = isSameHostNameAndProtocol(href) ? '_self' : '_blank';
  }

  token.attrPush(['target', targetValue]);
};

const overrideLinkRenderer = (md) => {
  const defaultRender = md.renderer.rules.link_open ||
    ((tokens, idx, options, _env, self) => self.renderToken(tokens, idx, options));

  md.renderer.rules.link_open = (tokens, idx, options, env, self) => {
    const token = tokens[idx];
    const hrefIndex = token.attrIndex('href');
    const href = token.attrs[hrefIndex][1];

    overrideLinkForRedirector(token, href, hrefIndex);
    assignTarget(token, href);

    return defaultRender(tokens, idx, options, env, self);
  };
};

const initMarkdown = (store) => {
  if (!markdownInternal) {
    markdownInternal = new MarkdownIt('zero').enable(['emphasis', 'image', 'link', 'list']);
    overrideLinkRenderer(markdownInternal);
  }

  $store = store;

  return markdownInternal;
};

export const markdown = ({ store, content }) => initMarkdown(store).render(content);

export const toPlainText = content => stripHtml(markdown({ store: {}, content }));

export default ({ store, content }) => markdown({ store, content });
