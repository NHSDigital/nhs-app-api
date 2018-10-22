const packageJson = require('../../package.json');

describe('package.json', () => {
  describe('dependencies', () => {
    it('will have no hat (^) versions for any dependency', () => {
      Object.keys(packageJson.dependencies).forEach((key) => {
        expect(packageJson.dependencies[key]).not.toContain('^');
      });
    });

    it('will have no hat (^) versions for any dev dependency', () => {
      Object.keys(packageJson.devDependencies).forEach((key) => {
        expect(packageJson.devDependencies[key]).not.toContain('^');
      });
    });
  });
});
