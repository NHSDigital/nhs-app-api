<template>
  <div>
    <dcr-error-no-access-gp-record
      v-if="showError"
      :has-errored="events.hasErrored"
      :has-access="events.hasAccess"
      :has-undetermined-access="events.hasUndeterminedAccess"/>
    <div v-else data-purpose="events">
      <div role="list" class="nhsuk-grid-row nhsuk-u-margin-bottom-4">
        <MedicalRecordCardGroupItem
          v-for="(event, index) in orderedEvents"
          :key="`event-${index}`"
          class="nhsuk-grid-column-full nhsuk-u-padding-bottom-2">
          <Card data-label="events">
            <div data-purpose="events-card">
              <p v-if="event.date"
                 class="nhsuk-u-font-weight-bold nhsuk-u-margin-bottom-0"
                 data-purpose="record-item-header">
                {{ event.date | datePart }}</p>
              <p v-else clss="nhsuk-u-margin-bottom-0"
                 data-purpose="record-item-header">
                {{ $t('myRecord.unknownDate') }}</p>
              <p class="nhsuk-u-margin-bottom-0"
                 data-purpose="record-item-detail">
                {{ event.locationAndDoneBy }}
              </p>

              <ul class="nhsuk-list nhsuk-list--bullet">
                <li v-for="(eventItem, eventItemIndex)
                      in event.eventItems"
                    :key="`line-${eventItemIndex}`">
                  {{ eventItem }}
                </li>
              </ul>
            </div>
          </Card>
        </MedicalRecordCardGroupItem>
      </div>
    </div>
    <glossary v-if="!showError"/>
    <desktopGenericBackLink v-if="!$store.state.device.isNativeApp"
                            :path="backPath"
                            :button-text="'generic.back'"
                            @clickAndPrevent="backButtonClicked"/>
  </div>
</template>

<script>
import orderBy from 'lodash/orderBy';
import Card from '@/components/widgets/card/Card';
import DcrErrorNoAccessGpRecord from '@/components/gp-medical-record/SharedComponents/DCRErrorNoAccessGpRecord';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import Glossary from '@/components/Glossary';
import MedicalRecordCardGroupItem from '@/components/gp-medical-record/SharedComponents/MedicalRecordCardGroupItem';
import ReloadRecordMixin from '@/components/gp-medical-record/ReloadRecordMixin';
import { GP_MEDICAL_RECORD_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    Card,
    DesktopGenericBackLink,
    MedicalRecordCardGroupItem,
    Glossary,
    DcrErrorNoAccessGpRecord,
  },
  mixins: [ReloadRecordMixin],
  data() {
    return {
      backPath: GP_MEDICAL_RECORD_PATH,
      resultsCollapsed: true,
      events: null,
    };
  },
  computed: {
    orderedEvents() {
      return orderBy((this.events || {}).data, [obj => obj.date], ['desc']);
    },
    showError() {
      return this.events &&
        (this.events.hasErrored
             || this.events.data.length === 0
             || !this.events.hasAccess);
    },
  },
  async mounted() {
    if (this.$store.state.myRecord.record.supplier !== 'TPP') {
      redirectTo(this, GP_MEDICAL_RECORD_PATH);
      return;
    }

    if (!this.$store.state.myRecord.record.tppDcrEvents) {
      await this.$store.dispatch('myRecord/load');
    }

    this.events = this.$store.state.myRecord.record.tppDcrEvents;
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.backPath);
    },
  },
};
</script>

<style module scoped lang="scss">
  @import "@/style/custom/inline-block-pointer";
</style>
