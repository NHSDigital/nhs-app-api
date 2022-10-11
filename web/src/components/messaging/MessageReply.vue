<template>
  <div :class="$style.messageReply">
    <div v-if="response" id="messageReplyResponseContainer" :class="$style.messageReplyResponseContainer">
      <h4>{{ $t('messages.messageReply.response.title') }}</h4>
      <p>{{ $t('messages.messageReply.response.reply', { response }) }}
        <formatted-date-time v-if="responseDate"
                             class="message-reply__formatted-time"
                             :date-time="responseDate"
                             :time-format="2"
        />.
      </p>
      <p>{{ $t('messages.messageReply.response.message', { senderName }) }}</p>
    </div>
    <div v-else-if="showOptions" :class="$style.messageReplyOptionsContainer">
      <div v-if="isCheckboxOptions" id="checkboxOptions">
        <h4>{{ $t('messages.messageReply.checkboxOption.title') }}</h4>
        <error-group :show-error="!isValidResponse">
          <error-message v-if="!isValidResponse" id="reply-checkbox-error">
            {{ $t('messages.appMessage.errors.checkbox.message') }}
          </error-message>
          <generic-checkbox
            :key="checkboxOption"
            name="replyoption"
            :value="checkboxOption"
            @input="onCheckboxChanged"
            @onCheckedChanged="selectedCheckboxValueChanged">
            <span>{{ checkboxOption }}</span>
          </generic-checkbox>
        </error-group>
      </div>
      <div v-if="options.length > 1" id="radioOptions">
        <nhs-uk-radio-group
          v-model="selectedRadioValue"
          :heading="$t('messages.messageReply.radioOptions.title')"
          :heading-as-html="true"
          :items="radioOptions"
          name="replyoptions"
          :render-as-html="true"
          :current-value="currentRadioChoice"
          :error="!isValidResponse"
          :error-text="$t('messages.appMessage.errors.radioButton.message')"
          @onselect="onRadioButtonChanged"
        />
      </div>
      <button
        id="replyButton"
        class="nhsuk-button"
        @click="onSendClicked">Send</button>
    </div>
    <div v-else>
      <button
        id="showKeywordReplies"
        :class="['nhsuk-button', $style['messageReply__reply']]"
        @click="onShowOptions">
        {{ $t('messages.messageReply.reply') }}
      </button>
    </div>
  </div>
</template>


<script>

import NhsUkRadioGroup from '@/components/nhsuk-frontend/NhsUkRadioGroup';
import FormattedDateTime from '@/components/widgets/FormattedDateTime';
import GenericCheckbox from '@/components/widgets/GenericCheckbox';
import { first, get } from 'lodash/fp';
import ErrorGroup from '@/components/ErrorGroup';
import ErrorMessage from '@/components/widgets/ErrorMessage';

export default {
  name: 'MessageReply',
  components: {
    GenericCheckbox,
    NhsUkRadioGroup,
    FormattedDateTime,
    ErrorGroup,
    ErrorMessage,
  },
  props: {
    messageReply: {
      type: Object,
      default: null,
    },
    senderName: {
      type: String,
      required: true,
    },
  },
  data() {
    return {
      selectedRadioValue: '',
      selectedCheckboxValue: '',
      showOptions: false,
      isValidResponse: true,
    };
  },
  computed: {
    options() {
      return this.messageReply.options;
    },
    radioOptions() {
      const replyOptions = [];

      this.options.forEach((opt) => {
        replyOptions.push({ value: opt.code, code: opt.code });
      });

      return replyOptions;
    },
    checkboxOption() {
      return get('code')(first(this.options));
    },
    checkboxOptions() {
      const replyOptions = [];

      this.options.forEach((opt) => {
        replyOptions.push({ label: opt.code, code: opt.code });
      });

      return replyOptions;
    },
    currentRadioChoice() {
      return this.selectedRadioValue;
    },
    response() {
      return this.messageReply.response;
    },
    responseDate() {
      return this.messageReply.responseSentDateTime;
    },
    isCheckboxOptions() {
      return this.options.length === 1;
    },
  },
  methods: {
    onShowOptions() {
      this.showOptions = !this.showOptions;
    },
    onSendClicked() {
      this.$emit('send_clicked',
        this.getResponseValue(),
        this.validateResponse(),
        this.isCheckboxOptions);
    },
    onRadioButtonChanged(value) {
      this.selectedRadioValue = value;
      this.$store.dispatch('pageLeaveWarning/shouldSkipDisplayingLeavingWarning', false);
    },
    onCheckboxChanged(value) {
      this.selectedCheckboxValue = (this.selectedCheckboxValue) ? '' : value;
    },
    validateResponse() {
      this.isValidResponse = this.getResponseValue() !== '';
      return this.isValidResponse;
    },
    getResponseValue() {
      if (this.isCheckboxOptions) return this.selectedCheckboxValue;

      return this.selectedRadioValue;
    },
    selectedCheckboxValueChanged(checked) {
      if (checked) {
        this.$store.dispatch('pageLeaveWarning/shouldSkipDisplayingLeavingWarning', false);
      } else {
        this.$store.dispatch('pageLeaveWarning/shouldSkipDisplayingLeavingWarning', true);
      }
    },
  },
};

</script>

<style module lang="scss" scoped>
@import '@/style/custom/message-reply';
</style>

<style>

/* overrides */

.nhsuk-checkboxes__item {
  margin-top: 15px;
}

#checkboxOptions .nhsuk-checkboxes__item:last-child, #checkboxOptions .nhsuk-checkboxes__item:last-of-type {
  margin-bottom: 15px;
}

.nhsuk-radios .nhsuk-radios__item {
  margin-bottom: 26px;
}
.message-reply__formatted-time {
    display: inline-block !important;
}
</style>
