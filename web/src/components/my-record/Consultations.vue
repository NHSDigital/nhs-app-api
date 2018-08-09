<template>
  <div :class="[$style.recordContent, getCollapseState]">
    <div v-if="data.hasErrored">
      <p> {{ $t('myRecord.genericErrorMessage') }} </p>
    </div>
    <div v-else>
      <div v-if="data.hasAccess">
        <div v-if="data.data.length > 0">
          <ul>
            <li v-for="(consultation, consultationIndex) in orderedConsultations"
                :key="`consultation-${consultationIndex}`" :class="$style.consultation">
              <label v-if="consultation.effectiveDate.value">
                {{ consultation.effectiveDate.value |
                datePart(consultation.effectiveDate.datePart) }}
              </label>
              <p :class="$style.consultationDetail"> {{ consultation.consultantLocation }} </p>
              <ul :class="$style.consultationLine">
                <li v-for="(consultationHeader, consultationHeaderIndex)
                    in consultation.consultationHeaders"
                    :key="`line-${consultationHeaderIndex}`" :class="$style.consultationLine">
                  <strong> {{ consultationHeader.header }} </strong>
                  <ul :class="$style.consultationTerm">
                    <li v-for="(consultationObservation, consultationObservationIndex)
                        in consultationHeader.observations"
                        :key="`line-${consultationObservationIndex}`">
                      {{ consultationObservation.term }}
                      <ul :class="$style.observationText">
                        <li v-for="(consultationObservationText, consultationObservationTextIndex)
                            in consultationObservation.associatedTexts"
                            :key="`line-${consultationObservationTextIndex}`">
                          {{ consultationObservationText }}
                        </li>
                      </ul>
                    </li>
                  </ul>
                </li>
              </ul>
            </li>
          </ul>
        </div>
        <div v-else>
          <p> {{ $t('myRecord.genericNoDataMessage') }} </p>
        </div>
      </div>
      <div v-else>
        <p> {{ $t('myRecord.genericNoAccessMessage') }} </p>
      </div>
    </div>
    <hr>
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
  },
};

</script>

<style lang="scss" module>
  @import '../../style/html';
  @import '../../style/fonts';
  @import '../../style/spacings';
  @import '../../style/colours';
  @import '../../style/elements';

  .recordContent { @include record-content };

  .consultation {
    border-bottom: 1px solid #e8edee;
    padding-bottom: 16px;
  }
  .consultationDetail {
    padding-bottom: 0px !important;
    padding-left: 16px;
  }
  .consultationLine {
    @include small_text;
    padding-left: 16px;
  }
  .consultationTerm {
    list-style-type: disc;
    list-style-position: inside;
    padding-left:16px;
    li::before {
      content: '';
      height:1px;
    }
    .observationText {
      li {
        list-style-type: none;
        padding-left:22px;
      }
      li::before{
        content: '- ';
      }
    }
  }
</style>
