<script>
import debounce from 'lodash/debounce';

export default {
  name: 'DebounceMixin',
  props: {
    clickDelay: {
      type: String,
      default: 'short',
      validator: value => ['none', 'short', 'medium', 'long'].indexOf(value) !== -1,
    },
  },
  data() {
    return {
      long: this.$store.app.$env.DEBOUNCE_LONG,
      medium: this.$store.app.$env.DEBOUNCE_MEDIUM,
      short: this.$store.app.$env.DEBOUNCE_SHORT,
    };
  },
  created() {
    if (this.clicked) {
      this.useDebounce(this.clicked, 'clicked');
    }
  },
  methods: {
    useDebounce(fn, prop) {
      if (this.clickDelay && this.clickDelay !== 'none') {
        const delay = this.$data[this.clickDelay];
        this[prop] = debounce(fn, delay, {
          leading: true,
          trailing: false,
        });
      }
    },
  },
};
</script>
