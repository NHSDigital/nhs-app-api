<template>
  <div v-if="showError" :class="[$style['record-content'], getCollapseState]"
       :aria-hidden="isCollapsed">
    <p v-if="data.hasErrored">
      {{ $t('my_record.genericErrorMessage') }}
    </p>
    <p v-else-if="!data.hasAccess">
      {{ $t('my_record.genericNoAccessMessage') }}
    </p>
    <p v-else>
      {{ $t('my_record.genericNoDataMessage') }}
    </p>
  </div>
  <div v-else :class="[$style['record-content'], getCollapseState]"
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
      <hr>
    </div>
  </div>
</template>

<script>

import _ from 'lodash';

export default {
  props: {
    isCollapsed: {
      type: Boolean,
      default: true,
    },
    data: {
      type: Object,
      default: () => {},
    },
  },
  computed: {
    getCollapseState() {
      return this.isCollapsed ? this.$style.closed : this.$style.opened;
    },
    orderedConsultations() {
      return _.orderBy(this.data.data, [obj => obj.effectiveDate.value], ['desc']);
    },
    showError() {
      return this.data.hasErrored
             || this.data.data.length === 0
             || !this.data.hasAccess;
    },
  },
};

</script>

<style module lang="scss" scoped>
 @import '../../style/medrecordcontent';

 .fieldName {
   padding-left: 1.3em;
   padding-right: 1.3em;
   padding-bottom: 0.250rem;
   color: #425563;
   font-size: 0.813em;
   font-weight: 700;
 }

</style>
