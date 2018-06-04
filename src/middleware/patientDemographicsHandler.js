/* eslint-disable */

export default function ({store, redirect }) {
  store.dispatch('myRecord/loadPatientDemographics').then(() => {
    if(store.state.myRecord.patientDemographics  === null) {
      redirect('/my-record/noaccess');
    }
  });
}
