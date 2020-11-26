<template>
  <fieldset class="nhsuk-fieldset" :aria-describedby="error ? `${name}-error` : undefined">
    <legend :class="['nhsuk-fieldset__legend', legendSize, 'nhsuk-u-margin-bottom-3']">
      <h1 ref="nhsukRadiosHeader"
          class="nhsuk-fieldset__heading nhsuk-u-margin-top-3"
          tabindex="-1">{{ heading }}</h1>
    </legend>

    <div role="alert" aria-atomic="true">
      <message-dialog v-if="error" message-type="error" :focusable="true">
        <message-text>
          {{ $t('messages.thereIsAProblem') }}
        </message-text>
        <message-list>
          <li>
            {{ errorText }}
          </li>
        </message-list>
      </message-dialog>
    </div>

    <div :class="['nhsuk-form-group', error ? 'nhsuk-form-group--error' : undefined]">
      <span v-if="error" :id="`${name}-error`" class="nhsuk-error-message">
        <span class="nhsuk-u-visually-hidden">{{ $t('generic.errorPrefix') }}</span>
        {{ errorText }}
      </span>

      <div class="nhsuk-radios">
        <div v-for="item in items" :key="`${name}-${item.value}`"
             class="nhsuk-radios__item">
          <input :id="`${name}-${item.value}`" v-model="choice" class="nhsuk-radios__input"
                 :name="name" type="radio" :value="item.value" :required="required">
          <label class="nhsuk-label nhsuk-radios__label" :for="`${name}-${item.value}`">
            {{ item.label || item.value }}
          </label>
        </div>
      </div>
    </div>
  </fieldset>
</template>

<script>
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageList from '@/components/widgets/MessageList';
import MessageText from '@/components/widgets/MessageText';
import LegendSize from '@/lib/legend-size';

export default {
  name: 'NhsUkRadioGroup',
  components: {
    MessageDialog,
    MessageList,
    MessageText,
  },
  props: {
    name: {
      type: String,
      required: true,
    },
    heading: {
      type: String,
      required: true,
    },
    legendSize: {
      type: String,
      default: LegendSize.ExtraLarge,
      validator: value => Object.values(LegendSize).includes(value),
    },
    error: {
      type: Boolean,
      default: false,
    },
    errorText: {
      type: String,
      default: undefined,
    },
    items: {
      type: Array,
      required: true,
    },
    required: {
      type: Boolean,
      default: true,
    },
    value: {
      type: String,
      default: undefined,
    },
  },
  data() {
    return {
      itemValues: this.items.map(v => v.value),
      choice: this.value,
    };
  },
  watch: {
    choice(to) {
      this.checkAndEmitIsValueValid(to);
      this.$emit('input', to);
    },
  },
  created() {
    this.checkAndEmitIsValueValid(this.choice);
  },
  mounted() {
    if (document.activeElement !== null) {
      document.activeElement.blur();
    }
    this.$refs.nhsukRadiosHeader.focus();
  },
  methods: {
    checkAndEmitIsValueValid(value) {
      const isValid = (!this.required && value === undefined) || this.itemValues.includes(value);
      this.$emit('validate', { isValid });
    },
  },
};
</script>
