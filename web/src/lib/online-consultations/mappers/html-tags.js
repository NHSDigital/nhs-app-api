import isString from 'lodash/fp/isString';

function mapHtmlTags(html) {
  if (!isString(html)) {
    return html;
  }

  let result = html;

  result = result.replace(/<ul/gi, '<ul class="nhsuk-list nhsuk-list--bullet"');

  result = result.replace(/<small/gi, '<span class="nhsuk-hint"');
  result = result.replace(/<\/small>/gi, '</span>');

  // This is temporary until eConsult change style names for care cards
  result = result.replace('nhsuk-care-card nhsuk-care-card--non-urgent', 'nhsuk-card nhsuk-card--care nhsuk-card--care--non-urgent');
  result = result.replace('nhsuk-care-card nhsuk-care-card--urgent', 'nhsuk-card nhsuk-card--care nhsuk-card--care--urgent');
  result = result.replace('nhsuk-care-card nhsuk-care-card--immediate', 'nhsuk-card nhsuk-card--care nhsuk-card--care--emergency');

  result = result.replace('nhsuk-care-card__heading-container', 'nhsuk-card--care__heading-container');

  result = result.replace('nhsuk-care-card__arrow', 'nhsuk-card--care__arrow');
  result = result.replace('nhsuk-care-card__content', 'nhsuk-card__content');
  return result;
}

export default mapHtmlTags;
