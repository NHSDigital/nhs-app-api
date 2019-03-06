<!-- eslint-disable vue/no-v-html -->
<template>
  <dcr-error-no-access v-if="showError"
                       :has-access="consultations.hasAccess"
                       :has-errored="consultations.hasErrored"
                       :is-collapsed="isCollapsed"
                       :class="[$style['record-content'], getCollapsedState]"
                       :aria-hidden="isCollapsed"/>
  <div v-else-if="!isCollapsed" :class="[$style['record-content'], getCollapsedState,
                                         !$store.state.device.isNativeApp && $style.desktopWeb]"
       :aria-hidden="isCollapsed">
    <div v-for="(consultation, consultationIndex) in orderedConsultations"
         :key="`consultation-${consultationIndex}`" :class="$style['record-item']"
         data-purpose="record-item">
      <span v-if="consultation.effectiveDate.value" :class="$style.fieldName">
        {{ consultation.effectiveDate.value | datePart(consultation.effectiveDate.datePart) }}
      </span>

      <p> {{ consultation.consultantLocation }} </p>

      <ul>
        <li v-for="(consultationHeader, consultationHeaderIndex)
            in consultation.consultationHeaders"
            :key="`line-${consultationHeaderIndex}`" :class="$style.consultationHeader">
          <strong> {{ consultationHeader.header }} </strong>

          <ul :class="$style.consultationTerm">
            <li v-for="(obsWithTerm, obsWithTermIndex)
                in consultationHeader.observationsWithTerm"
                :key="`line-${obsWithTermIndex}`">
              {{ obsWithTerm.term }}
              <ul :class="$style.observationText">
                <li v-for="(obsWithTermText, obsWithTermTextIndex)
                    in obsWithTerm.associatedTexts"
                    :key="`line-${obsWithTermTextIndex}`" v-html="obsWithTermText"/>
              </ul>
            </li>
          </ul>

          <ul :class="$style.consultationTerm">
            <li v-for="(associatedText, associatedTextIndex)
                in consultationHeader.associatedTexts"
                :key="`line-${associatedTextIndex}`">
              {{ associatedText }}
            </li>
          </ul>
        </li>
      </ul>
      <hr aria-hidden="true">
    </div>
  </div>
</template>

<script>
import _ from 'lodash';
import DcrErrorNoAccess from '@/components/my-record/SharedComponents/DCRErrorNoAccess';

export default {
  components: {
    DcrErrorNoAccess,
  },
  props: {
    isCollapsed: {
      type: Boolean,
      default: true,
    },
    consultations: {
      type: Object,
      default: () => {},
    },
  },
  computed: {
    getCollapsedState() {
      return this.isCollapsed ? this.$style.closed : this.$style.opened;
    },
    orderedConsultations() {
      return _.orderBy(this.consultations.data, [obj => obj.effectiveDate.value], ['desc']);
    },
    showError() {
      return this.consultations.hasErrored
             || this.consultations.data.length === 0
             || !this.consultations.hasAccess;
    },
  },
};

</script>

<style module lang="scss" scoped>
 @import '../../../style/medrecordcontent';
 @import '../../../style/medrecordtitle';

 .fieldName {
   padding-left: 1.3em;
   padding-right: 1.3em;
   padding-bottom: 0.250rem;
   color: #425563;
   font-size: 0.813em;
   font-weight: 700;
 }

 div {
  &.desktopWeb {
   max-width: 540px;

   span {
    font-family: $default_web;
    font-weight: normal;
   }
   p {
    font-family: $default_web;
    font-weight: normal;
   }
  }
 }
</style>
