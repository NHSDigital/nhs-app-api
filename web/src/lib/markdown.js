/* eslint-disable no-param-reassign */
import MarkdownIt from 'markdown-it';
import isEmpty from 'lodash/fp/isEmpty';
import { isSameHostNameAndProtocol, stripHtml } from '@/lib/utils';
import { REDIRECT_PARAMETER } from '@/router/names';
import { INTERSTITIAL_REDIRECTOR_PATH } from '@/router/paths';

let markdownInternal;
let $store;

const getTarget = (token, hrefIndex) => {
  const href = token.attrs[hrefIndex][1];

  if (href.startsWith('/') && !href.startsWith('//')) {
    return '_self';
  }

  return isSameHostNameAndProtocol(href) ? '_self' : '_blank';
};

const getHref = (token, hrefIndex) => {
  const href = token.attrs[hrefIndex][1];

  if (!isEmpty($store)) {
    const matchedService = $store.getters['knownServices/matchOneByUrl'](href) || {};
    if (matchedService.requiresAssertedLoginIdentity) {
      return `/${INTERSTITIAL_REDIRECTOR_PATH}?${REDIRECT_PARAMETER}=${encodeURIComponent(href)}`;
    }
  }

  return href;
};

const overrideLinkRenderer = (md) => {
  const defaultRender = md.renderer.rules.link_open ||
    ((tokens, idx, options, _env, self) => self.renderToken(tokens, idx, options));

  md.renderer.rules.link_open = (tokens, idx, options, env, self) => {
    const token = tokens[idx];
    const hrefIndex = token.attrIndex('href');
    const href = getHref(token, hrefIndex);
    const target = getTarget(token, hrefIndex);

    token.attrs[hrefIndex][1] = href;
    token.attrPush(['target', target]);

    return defaultRender(tokens, idx, options, env, self);
  };
};

const initMarkdown = (store) => {
  if (!markdownInternal) {
    markdownInternal = new MarkdownIt('zero').enable(['emphasis', 'image', 'link', 'list', 'newline']);
    overrideLinkRenderer(markdownInternal);
  }

  $store = store;

  return markdownInternal;
};

export const markdown = ({ store, content }) => initMarkdown(store).render(content);

export const toPlainText = content => stripHtml(markdown({ store: {}, content }));

export default ({ store, content }) => markdown({ store, content });
