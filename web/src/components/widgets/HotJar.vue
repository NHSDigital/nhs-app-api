<script>
export default {
  name: 'HotJar',
  metaInfo() {
    const head = {};
    if (this.$store.$env.HOTJAR_SITE_ID && this.$store.$env.HOTJAR_SITE_ID !== 'NOT_SET') {
      /* eslint-disable prefer-template */
      const hotJar = '(function (h, o, t, j, a, r) {' +
                   'h.hj = h.hj || function () { (h.hj.q = h.hj.q || []).push(arguments); };' +
                   'h._hjSettings = { hjid: ' + this.$store.$env.HOTJAR_SITE_ID + ', hjsv: 6 };' +
                   'a = o.getElementsByTagName(\'head\')[0];' +
                   'r = o.createElement(\'script\'); r.async = 1;' +
                   'r.src = t + h._hjSettings.hjid + j + h._hjSettings.hjsv;' +
                   'a.appendChild(r);' +
                   '}(window, document, \'https://static.hotjar.com/c/hotjar-\', \'.js?sv=\'));';

      head.script = [{
        innerHTML: hotJar,
      }];
    }

    return head;
  },
  render() {
    // Not functional.
    // Each component must have a template or implement render().
    return '';
  },
};
</script>
