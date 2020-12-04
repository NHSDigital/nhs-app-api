/* eslint-disable no-param-reassign */
import MarkdownIt from 'markdown-it';
import { isSameHostNameAndProtocol, stripHtml } from '@/lib/utils';

let markdownInternal;

const overrideLinkRenderer = (md) => {
  const defaultRender = md.renderer.rules.link_open ||
    ((tokens, idx, options, _env, self) => self.renderToken(tokens, idx, options));

  md.renderer.rules.link_open = (tokens, idx, options, env, self) => {
    const token = tokens[idx];
    const href = token.attrs[token.attrIndex('href')][1];
    let targetValue = '_blank';

    if (href.startsWith('/') && !href.startsWith('//')) {
      targetValue = '_self';
    } else {
      targetValue = isSameHostNameAndProtocol(href) ? '_self' : '_blank';
    }

    token.attrPush(['target', targetValue]);

    return defaultRender(tokens, idx, options, env, self);
  };
};

const initMarkdown = () => {
  if (markdownInternal) {
    return markdownInternal;
  }

  markdownInternal = new MarkdownIt('zero').enable(['emphasis', 'image', 'link', 'list']);

  overrideLinkRenderer(markdownInternal);

  return markdownInternal;
};

export const markdown = content => initMarkdown().render(content);

export const toPlainText = content => stripHtml(markdown(content));

export default content => markdown(content);
