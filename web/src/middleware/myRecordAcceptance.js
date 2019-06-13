import {
  MYRECORD,
  LEGACY_MYRECORDWARNING,
  MY_RECORD_VISION_DIAGNOSIS_DETAIL,
  MY_RECORD_VISION_EXAMINATIONS_DETAIL,
  MY_RECORD_VISION_PROCEDURES_DETAIL,
  MY_RECORD_VISION_TEST_RESULTS_DETAIL,
} from '@/lib/routes';

export default ({ redirect, route, store }) => {
  if (route.path.indexOf(LEGACY_MYRECORDWARNING.path) === 0) {
    redirect(MYRECORD.path);
    return;
  }

  if ((route.path.indexOf(MY_RECORD_VISION_EXAMINATIONS_DETAIL.path) === 0 ||
    route.path.indexOf(MY_RECORD_VISION_PROCEDURES_DETAIL.path) === 0 ||
    route.path.indexOf(MY_RECORD_VISION_TEST_RESULTS_DETAIL.path) === 0 ||
    route.path.indexOf(MY_RECORD_VISION_DIAGNOSIS_DETAIL.path) === 0)
    && !store.state.myRecord.hasAcceptedTerms) {
    redirect(MYRECORD.path);
    return;
  }

  if (route.path.indexOf(MYRECORD.path) > -1) {
    return;
  }

  if (store.state.myRecord.hasAcceptedTerms) {
    store.dispatch('myRecord/resetTerms');
  }
};
