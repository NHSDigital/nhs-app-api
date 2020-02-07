/* eslint-disable no-param-reassign */
import filter from 'lodash/fp/filter';
import get from 'lodash/fp/get';
import isEmpty from 'lodash/fp/isEmpty';
import keys from 'lodash/fp/keys';
import map from 'lodash/fp/map';
import glob from 'glob';

const exclusions = [
  '../src/components/organ-donation/EnsureDecisionMixin.vue',
  '../src/components/widgets/HotJar.vue',
  '../src/components/NativeOnlyMixin.vue',
  '../src/components/gp-medical-record/ReloadRecordMixin.vue',
  '../src/components/TermsConditionsMixin.vue',
  '../src/components/errors/ErrorPageMixin.vue',
];

const importComponent = (path) => {
  try {
    // eslint-disable-next-line
    const component = require(path);
    component.name = component.name || get('default.name')(component);
    return {
      component,
      path,
    };
  } catch (error) {
    return {
      error,
      path,
    };
  }
};

const processResults = (results) => {
  const names = {};

  return results.reduce((agg, result) => {
    if (result.error) {
      agg[result.error] = agg[result.error] || [];
      agg[result.error].push(result.path);
    } else if (!result.component) {
      agg['No error but component not loaded'] = agg['No error but component not loaded'] || [];
      agg['No error but component not loaded'].push(result.path);
    } else if (!get('component.name')(result)) {
      agg['Missing name'] = agg['Missing name'] || [];
      agg['Missing name'].push(result.path);
    } else if (names[result.component.name]) {
      agg['Duplicate name'] = agg['Duplicate name'] || [];
      agg['Duplicate name'].push(`${result.path}:  Name ${result.component.name} is already used by: ${names[result.component.name]}`);
    } else {
      names[result.component.name] = result.path;
    }
    return agg;
  }, {});
};

const createFailureMessage = (failures) => {
  let messages = [];
  messages.push('Analysis of all components failed as there are problems to resolve: ');
  messages.push('  In the list below, the following errors can be fixed like so:');
  messages.push('');
  messages.push('  No script available to transform:');
  messages.push('    Add the following to the component that has no script tag:');
  messages.push('      <script>');
  messages.push('        // This is required for JEST tests.  If omitted it results in the error');
  messages.push('        // No script available to transform');
  messages.push('        export default {};');
  messages.push('      </script>');
  messages.push('');
  messages.push('  Missing name:');
  messages.push('    Add a name to the component definition: ');
  messages.push('      export default {');
  messages.push('        name: \'HeaderSlim\',');
  messages.push('        components: {');
  messages.push('          ...');
  messages.push('        }');
  messages.push('      }');
  messages.push('');
  messages.push('  Failures:');

  messages = [
    ...messages,
    ...map(key => `  ${key}:\n    ${failures[key].join('\n    ')}\n`)(keys(failures)),
  ];

  return messages.join('\n');
};


describe('verify components', () => {
  it('must have a unique name and a script tag', (done) => {
    glob('src/components/**/*.vue', (err, files) => {
      const paths = filter(file => !exclusions.includes(file))(map(file => `../${file}`)(files));
      const results = map(importComponent)(paths);
      const failures = processResults(results);

      if (!isEmpty(failures)) {
        done.fail(createFailureMessage(failures));
      }

      done();
    });
  });
});
