import { Then } from 'cucumber';
import SymptomCheckerPage from '../page-objects/SymptomCheckerPage';
import { SYMPTOM_CHECKER_URL } from '../../../config/env';

Then('I should be on the symptom checker', function () {
  // For some unknown reason the symptom checker URL from the config always included a trailing
  // space which even the `trim` function would not remove - ABJ 2018-04-06
  const expected = `${SYMPTOM_CHECKER_URL.substring(0, SYMPTOM_CHECKER_URL.length - 1)}/`;
  const symptomCheckerPage = new SymptomCheckerPage({ world: this });

  return symptomCheckerPage.expect.url.toEqual(expected);
});
