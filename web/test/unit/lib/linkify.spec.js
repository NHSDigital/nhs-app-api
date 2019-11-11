import linkifyContent from '@/lib/linkify';

describe('linkify', () => {
  describe('linkifyContent', () => {
    describe('no linkable content', () => {
      let result;
      const html = 'Sample content without a link';

      beforeEach(() => {
        result = linkifyContent(html);
      });

      it('will return original content', () => {
        expect(result).toBe(html);
      });
    });

    describe('content with html tags', () => {
      let result;

      beforeEach(() => {
        result = linkifyContent('Sample <b>content</b> with html');
      });

      it('will return original content', () => {
        expect(result).toBe('Sample content with html');
      });
    });

    describe('content with `\n` break', () => {
      let result;

      beforeEach(() => {
        result = linkifyContent('Sample content with\n line breaks');
      });

      it('will return original content', () => {
        expect(result).toBe('Sample content with<br> line breaks');
      });
    });

    describe('content with an external url', () => {
      let result;

      beforeEach(() => {
        result = linkifyContent('Sample content with\n a link to http://www.test.com that works.');
      });

      it('will return content with link opening in a new window', () => {
        expect(result).toBe('Sample content with<br> a link to '
          + '<a href="http://www.test.com" target="_blank">http://www.test.com</a> that works.');
      });
    });

    describe('content with an internal url', () => {
      let result;

      beforeEach(() => {
        result = linkifyContent('Sample content with a link to http://localhost/foobar that works.');
      });

      it('will return content with link opening in the same window', () => {
        expect(result).toBe('Sample content with a link to <a href="/foobar" target="_self">'
        + 'http://localhost/foobar</a> that works.');
      });
    });

    describe('content with an partial url', () => {
      let result;

      beforeEach(() => {
        result = linkifyContent('Sample content with a link to foobar.com that works.');
      });

      it('will return content with link opening in a new window', () => {
        expect(result).toBe('Sample content with a link to <a href="https://foobar.com" target="_blank">'
        + 'foobar.com</a> that works.');
      });
    });

    describe('content with an email address', () => {
      let result;

      beforeEach(() => {
        result = linkifyContent('Sample content with a link to foo@bar.com that works.');
      });

      it('will return content with a mailto link', () => {
        expect(result).toBe('Sample content with a link to <a href="mailto:foo@bar.com" target="_blank">'
        + 'foo@bar.com</a> that works.');
      });
    });

    describe('content with multiple urls', () => {
      let result;

      beforeEach(() => {
        result = linkifyContent('Sample content with a link to foo.com and http://bar.com that works.');
      });

      it('will return content with multiple links', () => {
        expect(result).toBe('Sample content with a link to <a href="https://foo.com" target="_blank">'
        + 'foo.com</a> and <a href="http://bar.com" target="_blank">'
        + 'http://bar.com</a> that works.');
      });
    });
  });
});
