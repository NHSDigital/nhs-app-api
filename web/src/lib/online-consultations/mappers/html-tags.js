import isString from 'lodash/fp/isString';

function mapHtmlTags(html) {
  if (!isString(html)) {
    return html;
  }

  let result = html;

  result = result.replace(/<ul/gi, '<ul class="nhsuk-list nhsuk-list--bullet"');

  result = result.replace(/<small/gi, '<span class="nhsuk-hint"');
  result = result.replace(/<\/small>/gi, '</span>');

  return result;
}

export default mapHtmlTags;
