<template>
  <div>
    <div v-if="error && errorText">
      <span v-for="(singleError) in errorText"
            :id="`${id}error`" :key="singleError" class="nhsuk-error-message">
        <span class="nhsuk-u-visually-hidden">{{ $t('generic.input.errors.messagePrefix') }}</span>
        {{ singleError }}
      </span>
    </div>
    <generic-attachment :id="id"
                        :name="name"
                        :error="error"
                        :required="required"
                        :accept="accept"
                        @change="onSelectedFileChanged($event)"/>
  </div>
</template>

<script>
import GenericAttachment from '@/components/widgets/GenericAttachment';
import { questionAttachmentAnswerValid } from '@/lib/online-consultations/answer-validators';

export default {
  name: 'QuestionAttachment',
  components: {
    GenericAttachment,
  },
  props: {
    id: {
      type: String,
      default: undefined,
    },
    name: {
      type: String,
      default: undefined,
    },
    error: {
      type: Boolean,
      default: false,
    },
    errorText: {
      type: Array,
      default: undefined,
    },
    required: {
      type: Boolean,
      default: true,
    },
    accept: {
      type: Array,
      default: () => [],
    },
    maxSize: {
      type: Number,
      default: undefined,
    },
  },
  data() {
    return {
      file: undefined,
      attachmentValue: undefined,
    };
  },
  watch: {
    attachmentValue(to) {
      this.checkAndEmitIsValueValid(to);
      this.$emit('input', to);
    },
  },
  created() {
    if (process.client) {
      const reader = new FileReader();
      reader.onload = this.onFileLoad;
      reader.onerror = this.onFileError;
      reader.onabort = this.onFileAbort;

      this.reader = reader;
    }
    this.checkAndEmitIsValueValid(this.attachmentValue);
  },
  beforeDestroy() {
    if (this.reader) {
      this.reader.abort();
    }
  },
  methods: {
    checkAndEmitIsValueValid(value) {
      this.$emit('validate', questionAttachmentAnswerValid(value, this.required, this.accept, this.maxSize));
    },
    onSelectedFileChanged(event) {
      [this.file] = event.target.files;
      this.getFileAsBase64();
    },
    getFileAsBase64() {
      if (this.file !== undefined) {
        this.$store.dispatch('onlineConsultations/fileLoading');
        this.reader.readAsDataURL(this.file);
      } else {
        this.attachmentValue = undefined;
      }
    },
    onFileLoad() {
      this.$store.dispatch('onlineConsultations/fileLoadComplete');
      this.attachmentValue = {
        name: this.file.name,
        base64: this.reader.result.replace(`data:${this.file.type};base64,`, ''),
        type: this.file.type,
        size: this.file.size,
      };
    },
    onFileError() {
      this.$store.dispatch('onlineConsultations/fileLoadComplete');
      this.attachmentValue = undefined;
    },
    onFileAbort() {
      this.$store.dispatch('onlineConsultations/fileLoadComplete');
      this.attachmentValue = undefined;
    },
  },
};
</script>
