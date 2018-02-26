const expectation = page => ({
    toEqual: url => page.browser.assert.urlEquals(url),
});

export default expectation;