<script>
const KeyCodes = {
  NoKey: 0,
  TabKey: 9,
  LeftArrowKey: 38,
  DownArrowKey: 40,
};

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
        lastKey: KeyCodes.NoKey };
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
        if (e.keyCode === KeyCodes.TabKey ||
          (e.keyCode >= KeyCodes.LeftArrowKey && e.keyCode <= KeyCodes.DownArrowKey)) {
          this.highlight = true;
        }
        this.lastKey = KeyCodes.NoKey;
      },
      onKeyDown(e) {
        this.lastKey = e.keyCode;
      },
      onBlurButton(e) {
        this.highlight = false;
        if (e.relatedTarget === null &&
          (this.lastKey === KeyCodes.TabKey ||
            (this.lastKey >= KeyCodes.LeftArrowKey && this.lastKey <= KeyCodes.DownArrowKey))) {
          this.lastKey = KeyCodes.NoKey;
        }
      },
    },
  },
};
</script>
