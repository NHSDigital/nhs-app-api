import { Then } from 'cucumber';
import SymptomCheckerPage from '../page-objects/SymptomCheckerPage';

Then('I should be on the symptom checker', function () {
  const symptomCheckerPage = new SymptomCheckerPage({ world: this });

  return symptomCheckerPage.expect.url.toEqual('https://111.nhs.uk/');
});
