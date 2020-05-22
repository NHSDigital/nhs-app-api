import linkifyIt from 'linkify-it';
import { stripHtml } from '@/lib/utils';
import 'url-polyfill';

let linkifyInternal;

const initLinkify = () => {
  if (linkifyInternal) {
    return linkifyInternal;
  }

  linkifyIt.prototype.normalize = function normalize(match) {
    // eslint-disable-next-line no-param-reassign
    if (!match.schema) { match.url = `https://${match.url}`; }
    if (match.schema === 'mailto:' && !/^mailto:/i.test(match.url)) {
      // eslint-disable-next-line no-param-reassign
      match.url = `mailto:${match.url}`;
    }
  };

  linkifyInternal = linkifyIt();
  linkifyInternal.add('nhsapp:', 'http:');

  return linkifyInternal;
};

const replaceNewLines = content => content.replace(/\r?\n/g, '<br>');

export default (content) => {
  const sanitizedContent = stripHtml(content);
  const linkify = initLinkify();
  const matches = linkify.match(sanitizedContent);

  if (!matches) {
    return replaceNewLines(sanitizedContent);
  }

  const result = [];
  let last = 0;
  let target;
  let href;

  matches.forEach((match) => {
    if (last < match.index) {
      result.push(replaceNewLines(sanitizedContent.slice(last, match.index)));
    }

    if (match.url.startsWith('/') && !match.url.startsWith('//')) {
      target = '_self';
      href = match.url;
    } else {
      const matchUrl = new URL(match.url);
      target = (matchUrl.hostname === window.location.hostname) ? '_self' : '_blank';
      href = (matchUrl.hostname === window.location.hostname) ? matchUrl.pathname : match.url;
    }

    result.push(`<a href="${href}" target="${target}">${match.text}</a>`);
    last = match.lastIndex;
  });
  if (last < sanitizedContent.length) {
    result.push(replaceNewLines(sanitizedContent.slice(last)));
  }
  return result.join('');
};
