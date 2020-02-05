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
                {{ $t('my_record.noStartDate') }}</p>
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
                            :button-text="'rp03.backButton'"
                            @clickAndPrevent="backButtonClicked"/>
  </div>
</template>

<script>
import orderBy from 'lodash/orderBy';
import DesktopGenericBackLink from '../../components/widgets/DesktopGenericBackLink';
import MedicalRecordCardGroupItem from '@/components/gp-medical-record/SharedComponents/MedicalRecordCardGroupItem';
import Card from '@/components/widgets/card/Card';
import { MYRECORD } from '@/lib/routes';
import Glossary from '@/components/Glossary';
import { redirectTo } from '@/lib/utils';
import DcrErrorNoAccessGpRecord from '@/components/gp-medical-record/SharedComponents/DCRErrorNoAccessGpRecord';

export default {
  layout: 'nhsuk-layout',
  components: {
    Card,
    DesktopGenericBackLink,
    MedicalRecordCardGroupItem,
    Glossary,
    DcrErrorNoAccessGpRecord,
  },
  data() {
    return {
      backPath: MYRECORD.path,
      resultsCollapsed: true,
    };
  },
  computed: {
    orderedEvents() {
      return orderBy(this.events.data, [obj => obj.date], ['desc']);
    },
    showError() {
      return this.events.hasErrored
             || this.events.data.length === 0
             || !this.events.hasAccess;
    },
  },
  async asyncData({ store }) {
    if (!store.state.myRecord.record.tppDcrEvents) {
      await store.dispatch('myRecord/load');
    }
    return {
      events: store.state.myRecord.record.tppDcrEvents,
    };
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.backPath);
    },
  },
};
</script>

<style module scoped lang="scss">
a {
  display: inline-block;
  cursor: pointer;
}
</style>
