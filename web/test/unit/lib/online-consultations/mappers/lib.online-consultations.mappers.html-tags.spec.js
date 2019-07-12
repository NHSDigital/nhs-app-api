import mapHtmlTags from '@/lib/online-consultations/mappers/html-tags';
import each from 'jest-each';

describe('online consultations mappers html tags', () => {
  describe('html is not a string', () => {
    each([
      undefined,
      1,
      { something: 'random' },
      true,
      null,
    ]).it('will not manipulate the html', (html) => {
      // Act
      const parsedHtml = mapHtmlTags(html);

      // Assert
      expect(parsedHtml).toEqual(html);
    });
  });

  describe('html contains span elements and span text', () => {
    it('will replace only span tags with p tags and a class of nhsuk-body', () => {
      // Arrange
      const html = '<span>I am a span that contains span text.</span>';
      const expectedHtml = '<p class="nhsuk-body">I am a span that contains span text.</p>';

      // Act
      const parsedHtml = mapHtmlTags(html);

      // Assert
      expect(parsedHtml).toEqual(expectedHtml);
    });
  });

  describe('html contains ul elements and ul text', () => {
    it('will append nhsuk-list and nhsuk-list--bullet classes to ul tags', () => {
      // Arrange
      const html = '<ul>I am a ul that contains ul text.</ul>';
      const expectedHtml = '<ul class="nhsuk-list nhsuk-list--bullet">I am a ul that contains ul text.</ul>';

      // Act
      const parsedHtml = mapHtmlTags(html);

      // Assert
      expect(parsedHtml).toEqual(expectedHtml);
    });
  });

  describe('html contains small elements with small text', () => {
    it('will replace only small tags with span tags and a class of nhsuk-hint', () => {
      // Arrange
      const html = '<small>I am a small that contains small text.</small>';
      const expectedHtml = '<span class="nhsuk-hint">I am a small that contains small text.</span>';

      // Act
      const parsedHtml = mapHtmlTags(html);

      // Assert
      expect(parsedHtml).toEqual(expectedHtml);
    });
  });
});
