import LoginLayout from '@/layouts/login';
import { create$T } from '../helpers';

const $t = create$T(true);

describe('login layout', () => {
  describe('metaInfo', () => {
    let context;
    let head;

    beforeEach(() => {
      context = {
        header: 'pageHeaders.login',
        $t,
      };
      head = LoginLayout.metaInfo.call(context);
    });

    it('will set language from locale', () => {
      expect(head.htmlAttrs.lang).toBe('translate_language');
    });

    it('will set title to be the pageTitle', () => {
      expect(head.title).toBe('translate_pageTitles.login screen');
    });

    it('will have no scripts defined', () => {
      expect(head.script).toBeUndefined();
    });
  });
});
