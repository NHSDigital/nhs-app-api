export default {
  name: 'FormatDisplayName',
  methods: {
    getDisplayNameText(item) {
      if (item) {
        const trimmedItem = item.trim();
        if (trimmedItem) {
          return trimmedItem.toUpperCase();
        }
      }
      return '';
    },
  },
};
