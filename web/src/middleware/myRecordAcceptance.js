import { MYRECORD } from '@/lib/routes';

export default async ({ route, store }) => {
  if (route.path.indexOf(MYRECORD.path) > -1) return;
  if (store.state.myRecord.hasAcceptedTerms) store.dispatch('myRecord/resetTerms');
};
