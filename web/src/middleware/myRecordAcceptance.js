import { MYRECORD, LEGACY_MYRECORDWARNING } from '@/lib/routes';

export default async ({ redirect, route, store }) => {
  if (route.path.indexOf(LEGACY_MYRECORDWARNING.path) === 0) {
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
