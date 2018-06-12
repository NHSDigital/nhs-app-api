<template>
  <div id="mainDiv">
    <h5 :class="[$style.recordTitle, getCollapseState(isPatientDetailsCollapsed)]"
        @click="myRecordSectionClick(PATIENTDETAILS)">
      {{ $t('myRecord.patientInfo.sectionHeader') }}</h5>
    <patient-details :is-collapsed="isPatientDetailsCollapsed"
                     :patient-details="myRecord.patientDetails.data"/>

    <div v-if="myRecord.hasSummaryRecordAccess">
      <h5 :class="[$style.recordTitle, getCollapseState(isAllergiesAndAdverseReactionsCollapsed)]"
          @click="myRecordSectionClick(ALLERGIESANDADVERSEREACTIONS)">
        {{ $t('myRecord.allergiesAndAdverseReactions.sectionHeader') }}
      </h5>
      <allergies-and-adverse-reactions :is-collapsed="isAllergiesAndAdverseReactionsCollapsed"
                                       :data="myRecord.allergies"/>

      <h5 :class="[$style.recordTitle, getCollapseState(isAcuteMedicationsCollapsed)]"
          @click="myRecordSectionClick(ACUTEMEDICATIONS)">
        {{ $t('myRecord.acuteMedications.sectionHeader') }}
      </h5>
      <medications :is-collapsed="isAcuteMedicationsCollapsed"
                   :data="myRecord.medications.data.acuteMedications"
                   :has-error="myRecord.medications.hasErrored"/>

      <h5 :class="[$style.recordTitle, getCollapseState(isCurrentRepeatMedicationsCollapsed)]"
          @click="myRecordSectionClick(CURRENTREPEATMEDICATIONS)">
        {{ $t('myRecord.currentRepeatMedications.sectionHeader') }}
      </h5>
      <medications :is-collapsed="isCurrentRepeatMedicationsCollapsed"
                   :data="myRecord.medications.data.currentRepeatMedications"
                   :has-error="myRecord.medications.hasErrored"/>

      <h5 :class="[$style.recordTitle, getCollapseState(isDiscontinuedRepeatMedicationsCollapsed)]"
          @click="myRecordSectionClick(DISCONTINUEDREPEATMEDICATIONS)">
        {{ $t('myRecord.discontinuedRepeatMedications.sectionHeader') }}
      </h5>
      <medications :is-collapsed="isDiscontinuedRepeatMedicationsCollapsed"
                   :data="myRecord.medications.data.discontinuedRepeatMedications"
                   :has-error="myRecord.medications.hasErrored"/>

      <div v-if="myRecord.hasDetailedRecordAccess">
        <h5 :class="[$style.recordTitle, getCollapseState(isImmunisationsCollapsed)]"
            @click="myRecordSectionClick(IMMUNISATIONS)">
          {{ $t('myRecord.immunisations.sectionHeader') }}
        </h5>
        <immunisations :is-collapsed="isImmunisationsCollapsed"
                       :data="myRecord.immunisations"/>

        <h5 :class="[$style.recordTitle, getCollapseState(isTestResultsCollapsed)]"
            @click="myRecordSectionClick(TESTRESULTS)">
          {{ $t('myRecord.testResults.sectionHeader') }}
        </h5>
        <test-results :is-collapsed="isTestResultsCollapsed"
                      :data="myRecord.testResults"/>
      </div>
      <div v-else>
        <main :class="$style.content">
          <error-warning-dialog error-or-warning="warning">
            <p>
              {{ $t('myRecord.viewRestOfHealthRecordWarning') }}
            </p>
          </error-warning-dialog>
        </main>
      </div>
    </div>
    <div v-else>
      <main :class="$style.content">
        <error-warning-dialog error-or-warning="warning">
          <p>
            <b>{{ $t('myRecord.noRecordAccess.warningHeader') }}</b>
            <br>
            {{ $t('myRecord.noRecordAccess.warningBody') }}
          </p>
        </error-warning-dialog>
      </main>
    </div>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import PatientDetails from '@/components/my-record/PatientDetails';
import AllergiesAndAdverseReactions from '@/components/my-record/AllergiesAndAdverseReactions';
import ErrorWarningDialog from '@/components/errors/ErrorWarningDialog';
import Medications from '@/components/my-record/Medications';
import Immunisations from '@/components/my-record/Immunisations';
import TestResults from '@/components/my-record/TestResults';

const PATIENTDETAILS = 'patientdetails';
const ALLERGIESANDADVERSEREACTIONS = 'allergiesandadversereactions';
const ACUTEMEDICATIONS = 'acutemedications';
const CURRENTREPEATMEDICATIONS = 'currentrepeatmedications';
const DISCONTINUEDREPEATMEDICATIONS = 'discontinuedrepeatmedications';
const IMMUNISATIONS = 'immunisations';
const TESTRESULTS = 'testresults';

export default {
  middleware: ['auth', 'meta'],
  components: {
    ErrorWarningDialog,
    PatientDetails,
    AllergiesAndAdverseReactions,
    Medications,
    Immunisations,
    TestResults,
  },
  data() {
    return {
      PATIENTDETAILS,
      ALLERGIESANDADVERSEREACTIONS,
      ACUTEMEDICATIONS,
      CURRENTREPEATMEDICATIONS,
      DISCONTINUEDREPEATMEDICATIONS,
      IMMUNISATIONS,
      TESTRESULTS,
      isPatientDetailsCollapsed: false,
      isAllergiesAndAdverseReactionsCollapsed: true,
      isAcuteMedicationsCollapsed: true,
      isCurrentRepeatMedicationsCollapsed: true,
      isDiscontinuedRepeatMedicationsCollapsed: true,
      isImmunisationsCollapsed: true,
      isTestResultsCollapsed: true,
      myRecord: {
        hasSummaryRecordAccess: true,
        hasDetailedRecordAccess: true,
        patientDetails: {
          hasAccess: true,
          hasErrored: false,
          errors: '',
          data: {
            title: 'Mr',
            firstName: 'Test',
            surname: 'Tester',
            callingName: 'Test Tester 1',
            nhsNumber: '123456789',
            dateOfBirth: '1994-02-21T00:00:00',
            sex: 'Male',
            contactDetails: {
              telephoneNumber: '00000 111111',
              mobileNumber: '',
              emailAddress: '',
            },
            address: {
              line1: '14',
              line2: 'Test street',
              line3: 'Test Village',
              town: 'Test Town',
              county: 'Test County',
              postcode: 'BT3 3XY',
            },
          },
        },
        allergies: {
          hasAccess: false,
          hasErrored: false,
          errors: '',
          data:
            [
              { name: 'test', symptom: 'sympton 1', date: '2003-02-21T00:00:00' },
              { name: 'another test', symptom: 'symptom 2', date: '2010-02-21T00:00:00' },
            ],
        },
        medications: {
          hasAccess: true,
          hasErrored: false,
          errors: '',
          data: {
            acuteMedications: [
              {
                date: '2003-02-21T00:00:00',
                lineItems:
                [
                  {
                    text: 'Mixture Drug Name 250mg capsules', lineItems: [],
                  },
                  {
                    text: 'Mixture Name consisting of:',
                    lineItems:
                      [
                        'Constituent Item 1 - 50mg',
                        'Constituent Item 2 - 100mg',
                      ],
                  },
                  {
                    text: 'One to Be Taken Four Times A Day', lineItems: [],
                  },
                  {
                    text: '28 capsules', lineItems: [],
                  },
                  {
                    text: 'Ended:  5 June 2018', lineItems: [],
                  },
                ],
              },
              {
                date: '2013-02-21T00:00:00',
                lineItems:
                [
                  {
                    text: 'Simple Drug Name 10mg capsules', lineItems: [],
                  },
                  {
                    text: 'Two to Be Taken Three Times A Day', lineItems: [],
                  },
                  {
                    text: '12 capsules', lineItems: [],
                  },
                ],
              },
            ],
            currentRepeatMedications: [
              {
                date: '2012-03-21T00:00:00',
                lineItems:
                [
                  {
                    text: 'Mixture Drug Name 250mg capsules', lineItems: [],
                  },
                  {
                    text: 'Mixture Name consisting of:',
                    lineItems:
                      [
                        'Constituent Item 1 - 50mg',
                        'Constituent Item 2 - 100mg',
                      ],
                  },
                  {
                    text: 'One to Be Taken Four Times A Day', lineItems: [],
                  },
                  {
                    text: '28 capsules', lineItems: [],
                  },
                  {
                    text: 'Ended:  5 June 2018', lineItems: [],
                  },
                ],
              },
              {
                date: '2013-02-21T00:00:00',
                lineItems:
                [
                  {
                    text: 'Another Simple Drug Name 10mg capsules', lineItems: [],
                  },
                  {
                    text: 'Two to Be Taken Three Times A Day', lineItems: [],
                  },
                  {
                    text: '12 capsules', lineItems: [],
                  },
                ],
              },
              {
                date: '2013-02-23T00:00:00',
                lineItems:
                [
                  {
                    text: 'Simple Drug Name 30mg capsules', lineItems: [],
                  },
                  {
                    text: 'Two to Be Taken Twice Times A Day', lineItems: [],
                  },
                  {
                    text: '35 capsules', lineItems: [],
                  },
                ],
              },
            ],
            discontinuedRepeatMedications: [
              {
                date: '2008-02-21T00:00:00',
                lineItems:
                [
                  {
                    text: 'Mixture Drug Name 989mg capsules', lineItems: [],
                  },
                  {
                    text: 'Mixture Name consisting of:',
                    lineItems:
                      [
                        'Constituent Item 1 - 50mg',
                        'Constituent Item 2 - 23mg',
                      ],
                  },
                  {
                    text: 'One to Be Taken Eight Times A Day', lineItems: [],
                  },
                  {
                    text: '89 capsules', lineItems: [],
                  },
                  {
                    text: 'Ended:  5 June 2018', lineItems: [],
                  },
                ],
              },
              {
                date: '2014-02-21T00:00:00',
                lineItems:
                [
                  {
                    text: 'Another Simple Drug Name 30mg capsules', lineItems: [],
                  },
                  {
                    text: 'One to Be Taken Three Times A Day', lineItems: [],
                  },
                  {
                    text: '100 capsules', lineItems: [],
                  },
                ],
              },
            ],
          },
        },
        immunisations: {
          hasAccess: true,
          hasErrored: false,
          errors: '',
          data: [
            { date: '2003-02-21T00:00:00', term: 'First meningitis C vaccination' },
            { date: '2018-07-14T00:00:00', term: 'Chicken pox shots' },
          ],
        },
        testResults: {
          hasAccess: true,
          hasErrored: false,
          errors: '',
          data: [{
            date: '2014-02-21T00:00:00',
            term: 'O/E –   weight:  67 Kg',
            lineItems: [],
          },
          {
            date: '2014-02-21T00:00:00',
            term: 'Neutrophil count:  5.58 x10^9/L  (normal range:  1.7 – 6)',
            lineItems: [],
          },
          {
            date: '2018-05-31T00:00:00',
            term: 'O/E - blood pressure reading',
            lineItems: [
              'Systolic blood pressure:  124 mmHg',
              'Diastolic blood pressure:  78 mmHg',
            ],
          },
          {
            date: '2014-02-21T00:00:00',
            term: 'Urea and electrolytes',
            lineItems: [
              'Serum sodium:  140 mmol/L  (normal range:  133 – 146)',
              'Serum potassium:  4.1 mmol/L  (normal range:  3.5 –   5.3',
              ' Serum urea level:  5.6 mmol/L  (normal range:  2.5 –   7.',
              'Serum creatinine:  92 umol/L  (normal range:  64 -  104',
              'GFR calculated abbreviated MDRD:  79 mL/min/1.72m*2',
            ],
          }],
        },
      },
    };
  },
  methods: {
    getCollapseState(collapsed) {
      return collapsed ? this.$style.closed : this.$style.opened;
    },
    myRecordSectionClick(section) {
      switch (section) {
        case PATIENTDETAILS:
          this.isPatientDetailsCollapsed = !this.isPatientDetailsCollapsed;
          break;
        case ALLERGIESANDADVERSEREACTIONS:
          this.isAllergiesAndAdverseReactionsCollapsed =
          !this.isAllergiesAndAdverseReactionsCollapsed;
          break;
        case ACUTEMEDICATIONS:
          this.isAcuteMedicationsCollapsed =
          !this.isAcuteMedicationsCollapsed;
          break;
        case CURRENTREPEATMEDICATIONS:
          this.isCurrentRepeatMedicationsCollapsed =
          !this.isCurrentRepeatMedicationsCollapsed;
          break;
        case DISCONTINUEDREPEATMEDICATIONS:
          this.isDiscontinuedRepeatMedicationsCollapsed =
          !this.isDiscontinuedRepeatMedicationsCollapsed;
          break;
        case IMMUNISATIONS:
          this.isImmunisationsCollapsed =
            !this.isImmunisationsCollapsed;
          break;
        case TESTRESULTS:
          this.isTestResultsCollapsed =
            !this.isTestResultsCollapsed;
          break;
        default:
          break;
      }
    },
  },
};

</script>

<style module lang="scss">
  @import '../../style/html';
  @import '../../style/fonts';
  @import '../../style/spacings';

  .recordTitle {
    font-family: "FrutigerLTW01-65Bold", Arial, sans-serif;
    font-weight: 700;
    font-size: 18px;
    line-height: 23px;
    color: #005EB8;
    padding: 16px;
    padding-right: 30px;
    box-sizing: border-box;
    background: transparent url('~/assets/icon_arrow_left.svg') no-repeat center right;
    background-position: right 16px center;
    transition: ease 0.5s;
    border-bottom: 2px solid $white;
    margin-bottom: 0px !important;

    &.opened {
      background: transparent url('~/assets/icon_arrow_down.svg') no-repeat center right;
      background-position: right 16px center;
    }
    &.closed {
      background: transparent url('~/assets/icon_arrow_left.svg') no-repeat center right;
      background-position: right 16px center;
    }
  }

</style>
