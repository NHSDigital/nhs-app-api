<script>
import { key } from '@/lib/utils';

export default {
  name: 'TabFocusMixin',
  tabMixin: {
    directives: {
      tabbing: {
        // directive definition
        /* eslint no-param-reassign:
        ["error", { "props": true, "ignorePropertyModificationsFor": ["vnode"] }] */
        bind(el, binding, vnode) {
          vnode.context.stylingBinding = binding.value;
          el.addEventListener('keyup', vnode.context.onFocusButton);
          el.addEventListener('keydown', vnode.context.onKeyDown);
          el.addEventListener('blur', vnode.context.onBlurButton);
        },
      },
    },
    data() {
      return {
        stylingBinding: null,
        highlight: false,
        lastKey: '' };
    },
    computed: {
      getStyleClasses() {
        let styleClasses = [];
        if (this.stylingBinding) {
          styleClasses = this.stylingBinding;
          const index = styleClasses.indexOf(this.$style.tabFocus);
          if (index > -1) {
            styleClasses.splice(index, 1);
          }
        } else {
          styleClasses.push(this.defaultClasses);
        }

        if (this.highlight) {
          styleClasses.push(this.$style.tabFocus);
        }

        return styleClasses;
      },
    },
    methods: {
      onFocusButton(e) {
        if (e.key === key.Tab ||
          (e.key >= key.ArrowLeft && e.key <= key.ArrowDown)) {
          this.highlight = true;
        }
        this.lastKey = '';
      },
      onKeyDown(e) {
        this.lastKey = e.key;
      },
      onBlurButton(e) {
        this.highlight = false;
        if (e.relatedTarget === null &&
          (this.lastKey === key.Tab ||
            (this.lastKey >= key.ArrowLeft && this.lastKey <= key.ArrowDown))) {
          this.lastKey = '';
        }
      },
    },
  },
};
</script>
