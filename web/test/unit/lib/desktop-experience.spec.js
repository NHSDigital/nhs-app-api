import {
  getDynamicStyleName,
  getDynamicStyle,
  exchangeStyle,
} from '@/lib/desktop-experience';

describe('desktop-experience', () => {
  let context;

  beforeEach(() => {
    context = {
      $store: {
        state: {
          device: {
            isNativeApp: true,
          },
        },
      },
      $style: {
        'some-style': 'resolved-some-style',
        'another-style': 'resolved-another-style',
        'some-style-desktop': 'resolved-some-style-desktop',
        'another-style-desktop': 'resolved-another-style-desktop',
      },
    };
  });

  describe('getDynamicStyleName', () => {
    it('will suffix the class with -desktop with a string arg.', () => {
      context.$store.state.device.isNativeApp = false;
      expect(getDynamicStyleName(context, 'some-style')).toEqual(['some-style-desktop']);
    });

    it('will suffix the class with -desktop with an array of strings as an arg', () => {
      context.$store.state.device.isNativeApp = false;

      expect(getDynamicStyleName(context, [
        'some-style',
        'another-style',
      ])).toEqual([
        'some-style-desktop',
        'another-style-desktop',
      ]);
    });

    it('will not suffix the class with -desktop with a string arg', () => {
      context.$store.state.device.isNativeApp = true;
      expect(getDynamicStyleName(context, 'some-style')).toEqual(['some-style']);
    });

    it('will not suffix the class with -desktop with an array of strings as an arg', () => {
      context.$store.state.device.isNativeApp = false;

      expect(getDynamicStyleName(context, [
        'some-style',
        'another-style',
      ])).toEqual([
        'some-style-desktop',
        'another-style-desktop',
      ]);
    });
  });

  describe('getDynamicStyle', () => {
    it('will suffix the class with -desktop with a string arg and map as style.', () => {
      context.$store.state.device.isNativeApp = false;
      expect(getDynamicStyle(context, 'some-style')).toEqual(['resolved-some-style-desktop']);
    });

    it('will suffix the class with -desktop with an array of strings as an arg', () => {
      context.$store.state.device.isNativeApp = false;

      expect(getDynamicStyle(context, [
        'some-style',
        'another-style',
      ])).toEqual([
        'resolved-some-style-desktop',
        'resolved-another-style-desktop',
      ]);
    });

    it('will not suffix the class with -desktop with a string arg and map as style.', () => {
      context.$store.state.device.isNativeApp = true;
      expect(getDynamicStyle(context, 'some-style')).toEqual(['resolved-some-style']);
    });

    it('will not suffix the class with -desktop with an array of strings as an arg.', () => {
      context.$store.state.device.isNativeApp = false;

      expect(getDynamicStyle(context, [
        'some-style',
        'another-style',
      ])).toEqual([
        'resolved-some-style-desktop',
        'resolved-another-style-desktop',
      ]);
    });
  });


  describe('exchangeStyle', () => {
    it('will swap a style if mapping is not found and use the default style resolver.', () => {
      expect(exchangeStyle({})('some-style')).toEqual('some-style-desktop');
    });

    it('will swap a style if mapping is found and use the default style resolver.', () => {
      expect(exchangeStyle({ 'some-style': 'another-style' })('some-style')).toEqual('another-style');
    });
  });
});
