<template>
  <div v-if="showTemplate"
       id="mainDiv"
       :class="[$style['pull-content'], !$store.state.device.isNativeApp && $style.desktopWeb]">
    <message-dialog message-type="warning" icon-text="Important">
      <message-text :class="$style.warningText">
        {{ $t('my_record.personalRecordText.warningText.wt1') }}
      </message-text>
      <message-text :class="$style.warningText">
        {{ $t('my_record.personalRecordText.warningText.wt2') }}
      </message-text>
    </message-dialog>
    <div :class="$style.info" data-purpose="info">
      <p >{{ $t('my_record.personalRecordText.body') }}</p>
      <p>{{ $t('my_record.personalRecordText.bulletPointHeader') }}</p>
      <ul>
        <li>{{ $t('my_record.personalRecordText.bulletPoints.bp1') }}</li>
        <li>{{ $t('my_record.personalRecordText.bulletPoints.bp2') }}</li>
      </ul>
    </div>
    <form :action="myRecordPath" method="get">
      <input :value="JSON.stringify({ myRecord: { hasAcceptedTerms: true }})"
             type="hidden"
             name="nojs">

      <generic-button :class="[$style.button, $style.green]"
                      @click="onContinueButtonClicked">
        {{ $t('my_record.personalRecordText.agreeButtonText') }}
      </generic-button>
    </form>
    <form v-if="$store.state.device.isNativeApp" :action="indexPath" method="get">
      <generic-button :class="[$style.button, $style.grey]"
                      @click="onBackButtonClicked">
        {{ $t('my_record.personalRecordText.backButtonText') }}
      </generic-button>
    </form>

    <desktopGenericBackLink
      v-else
      :path="indexPath"
      :button-text="'my_record.personalRecordText.backButtonText'"
      @clickAndPrevent="onBackButtonClicked"/>
  </div>
</template>

<script>
import GenericButton from '@/components/widgets/GenericButton';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import { INDEX, MYRECORD } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';
import DesktopGenericBackLink from '../../components/widgets/DesktopGenericBackLink';

export default {
  name: 'Warning',
  components: {
    GenericButton,
    MessageDialog,
    MessageText,
    DesktopGenericBackLink,
  },
  props: {
    id: {
      type: String,
      default: '',
    },
  },
  data() {
    return {
      indexPath: INDEX.path,
      myRecordPath: MYRECORD.path,
    };
  },
  methods: {
    onContinueButtonClicked(event) {
      event.preventDefault();
      sessionStorage.setItem('hasAgreedToMedicalWarning', true);
      this.$store.dispatch('myRecord/acceptTerms');
      this.$store.dispatch('myRecord/load');
    },
    onBackButtonClicked(event) {
      event.preventDefault();
      redirectTo(this, this.indexPath, null);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../style/info';
  @import '../../style/fonts';
  @import '../../style/buttons';
  @import '../../style/textstyles';
  @import '../../style/fonts';

  .h2 {
    display: block;
    font-family: $frutiger-bold;
    font-weight: 700;
    font-size: 1.125em;
    line-height: 1.125em;
    color: #425563;
    letter-spacing: -0.063em;
    padding-bottom: 0.5em;
    padding-top: 0.5em;
  }

  div {
    &.desktopWeb {
      max-width: 540px;

      h2 {
        font-family: $default_web;
        font-weight: bold;
        font-size: 1.375em;
        line-height: 1.375em;
        letter-spacing: 0.5px;
      }

      .warningText {
        font-family: $default_web;
        font-weight: normal;
      }

      li {
        font-family: $default_web;
        font-weight: normal;
      }

      p {
        font-family: $default_web;
        font-weight: normal;
      }

      .button {
        @include webButton;
        box-sizing: border-box;
        padding: 0.625em;
        background-color: $nhs_blue;
        border: none;
        border-radius: 0.125em;
        outline: none;
        transition: all ease 0.5s;
        cursor: pointer;
        width: auto;
        min-width: 12.5em;
        padding-left: 2em;
        padding-right: 2em;
        max-width: 960px;
        display: block;
        width: auto;

        :focus {
          outline-color: $focus_highlight;
          box-shadow: inset 0 0 0 4px $focus_highlight;
          outline-offset: -5px;
        }

        &.green {
          background-color: $light_green;
          box-shadow: 0 0.125em 0 0 $dark_green;
        }

        &.green:focus {
          outline-color: $focus_highlight;
          box-shadow: inset 0 0 0 4px $focus_highlight;
          outline-offset: -5px;
        }

        &.green:hover {
          outline-color: $focus_highlight;
          box-shadow: inset 0 0 0 4px $focus_highlight;
          outline-offset: -5px;
        }
      }
    }
  }
</style>
