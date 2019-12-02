<template>
  <div>
    <scr-error-no-access-gp-record
      v-if="showError"
      :has-errored="medicines.hasErrored"
      :has-undetermined-access="medicines.hasUndeterminedAccess"/>
    <div
      v-else
      role="list"
      class="nhsuk-grid-row nhsuk-u-margin-bottom-4">
      <MedicalRecordCardGroupItem
        v-for="(item, index) in orderedMedicines"
        :key="`medicine-${index}`"
        data-purpose="record-item"
        class="nhsuk-grid-column-full nhsuk-u-padding-bottom-2">
        <Card data-label="medicines">
          <div data-purpose="medicines-card">
            <p v-if="item.date"
               class="nhsuk-u-margin-bottom-0 nhsuk-u-font-weight-bold">
              {{ item.date | datePart }}
            </p>
            <p v-else
               class="nhsuk-u-margin-bottom-0 nhsuk-u-font-weight-bold">
              {{ $t('my_record.noStartDate') }}
            </p>
            <div v-for="(lineItem, lineIndex) in item.lineItems"
                 :key="`line-${lineIndex}`">
              <p class="nhsuk-u-margin-bottom-0">
                {{ lineItem.text }}
              </p>
              <ul v-if="lineItem.lineItems.length">
                <li v-for="(innerLineItem, innerLineItemIndex) in lineItem.lineItems"
                    :key="`innerline-${innerLineItemIndex}`">
                  {{ innerLineItem }}
                </li>
              </ul>
            </div>
          </div>
        </Card>
      </MedicalRecordCardGroupItem>
    </div>
  </div>
</template>

<script>
import orderBy from 'lodash/fp/orderBy';
import ScrErrorNoAccessGpRecord from '@/components/gp-medical-record/SharedComponents/SCRErrorNoAccessGpRecord';
import MedicalRecordCardGroupItem from '@/components/gp-medical-record/SharedComponents/MedicalRecordCardGroupItem';
import Card from '@/components/widgets/card/Card';

export default {
  name: 'Medicines',
  components: {
    Card,
    ScrErrorNoAccessGpRecord,
    MedicalRecordCardGroupItem,
  },
  props: {
    medicines: {
      type: Array,
      default: () => [],
    },
    showError: {
      type: Boolean,
      default: false,
    },
  },
  computed: {
    orderedMedicines() {
      return orderBy([item => item.date], ['desc'])(this.medicines);
    },
  },
};
</script>

<style module scoped lang="scss">
@import "../../../style/colours";
@import "../../../style/desktopWeb/accessibility";
</style>
