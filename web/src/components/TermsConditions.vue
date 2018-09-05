<template>
  <div v-if="showTemplate">
    <div v-if="hasTriedToContinue && !areTermsAccepted" id="error_msg">
      <message-dialog :class="$style.customErrorBox" message-type="error" icon-text="Error">
        <p :class="$style.customErrorText"> {{ $t('termsAndConditions.errorMsgHeader') }} </p>
        <ul>
          <li> {{ $t('termsAndConditions.errorMsgText') }} </li>
        </ul>
      </message-dialog>
    </div>
    <div id="text_body" :class="$style.info">
      <p> {{ $t('termsAndConditions.body1') }}
        <a href="">{{ $t('termsAndConditions.link1') }}</a>,
        <a href="">{{ $t('termsAndConditions.link2') }}</a> and
        <a href="">{{ $t('termsAndConditions.link3') }}</a>.
        {{ $t('termsAndConditions.body2') }} </p>
      <p> {{ $t('termsAndConditions.listTitle') }} </p>
      <ul>
        <li> {{ $t('termsAndConditions.listItem1') }} </li>
        <li> {{ $t('termsAndConditions.listItem2') }} </li>
        <li> {{ $t('termsAndConditions.listItem3') }} </li>
      </ul>
      <p> {{ $t('termsAndConditions.body3') }} </p>
    </div>
    <div :class="getErrorState()">
      <error-message v-if="hasTriedToContinue && !areTermsAccepted"
                     id="error_txt"
                     :class="$style.validationText">
        {{ $t('termsAndConditions.checkBoxError') }}
      </error-message>
      <div :class="$style['checkbox-panel']">
        <div id="agree_checkbox" class="clickme" @click="check">
          <checked-icon :selected="areTermsAccepted"/>
        </div>
        <input
          id="hiddenCheckbox"
          :value="areTermsAccepted"
          :class="$style.hideDefaultCheckbox"
          :checked="check"
          v-model="areTermsAccepted"
          type="checkbox"
          name="termsAndConditions">
        <label for="hiddenCheckbox" @click="check">
          {{ $t('termsAndConditions.checkBoxText') }}
        </label>
      </div>
    </div>
    <button id="btn_accept" :class="[$style.button, $style.green]"
            @click="onConfirmButtonClicked">
      {{ $t('termsAndConditions.btnAccept') }}
    </button>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import moment from 'moment';
import CheckedIcon from '../components/icons/CheckedIcon';
import GenericButton from '../components/widgets/GenericButton';
import ErrorMessage from './widgets/ErrorMessage';
import MessageDialog from '../components/widgets/MessageDialog';

export default {
  components: {
    ErrorMessage,
    CheckedIcon,
    GenericButton,
    MessageDialog,
  },
  data() {
    return {
      areTermsAccepted: false,
      hasTriedToContinue: false,
    };
  },
  methods: {
    check() {
      this.areTermsAccepted = !this.areTermsAccepted;
    },
    onConfirmButtonClicked() {
      this.hasTriedToContinue = true;
      if (this.areTermsAccepted) {
        const consentRequest = {
          ConsentGiven: true,
          DateOfConsent: moment().format(),
        };
        const { authResponse } = this.$route.params;
        const message = {
          a: { consentRequest },
          b: authResponse,
        };
        this.$store.dispatch('auth/goHandleAuthResponse', message);
      }
    },
    getErrorState() {
      if (this.hasTriedToContinue && !this.areTermsAccepted) {
        return this.$style.validationBorderLeft;
      }
      return null;
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../style/buttons";
  @import "../style/forms";
  @import "../style/home";

  p {
    padding-bottom: 0.5em;
    padding-top: 0.5em;
    display: block;
    font-weight: normal;
    line-height: 1.5em;
  }
  a {
    color: #005EB8;
    font-size: 1em;
    line-height: 1em;
    font-weight: bold;
    text-decoration: none;
    vertical-align: middle;
  }
  .hideDefaultCheckbox {
    display: none;
  }

  .info {
    font-size: 1em;
    margin-bottom: 0.5em;
    ul li {
      margin-left: 2em;
      padding-bottom: 0.5em;
      padding-top: 0.5em;
    }
    p a {
      display: inline-block;
      text-decoration: underline;
    }
    a {
      padding-bottom: 0.5em;
      padding-top: 0.5em;
    }
  }

  ul {
    display: block;
    list-style-type: disc;
  }

  label {
    padding-top: 0.125em;
    padding-bottom: 0.5em;
  }
  button {
    -webkit-font-smoothing: antialiased;
  }
  .validationBorderLeft {
    border-left: 4px #DA291C solid !important;
    padding-left: 1em !important;
    margin-top: 1em;
    margin-bottom: 0.5em;
  }
  .customErrorText {
    padding: 1em;
    padding-bottom: 0.150em;
    width: 100%;
    display: block;
    font-weight: normal;
    font-size: 1.125em;
    line-height: 1.125em;
    color: #212B32;
  }

  .customErrorBox {
    width: 100%;
    height: auto;
    background-color: #ffffff;
    border-top: 0.25em #005EB8 solid;
    margin-bottom: 1em;
    margin-top: 2.125em;
    padding-bottom: 0.850em;
    ul li {
      padding-top: 0.25em;
      display: block;
      font-weight: normal;
      font-size: 1.125em;
      line-height: 1.125em;
      color: #DA291C;

    }
    ul {
      list-style: none;
      padding: 0em 1em 0em 1em;
    }
  }
  .validationText {
    color: #DA291C !important;
    font-weight: 700 !important;
    padding-bottom: 0em !important;
  }

</style>
