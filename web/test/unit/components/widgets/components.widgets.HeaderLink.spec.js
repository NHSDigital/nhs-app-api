import HeaderLink from '@/components/widgets/HeaderLink';
import { mount } from '../../helpers';

const additionalInternalAnchorAttrs = { target: '_blank', rel: 'noopener noreferrer' };

const anchorId = 'Id';
const anchorName = 'Name';
const anchorValue = '/value';
const lastPlaceLink = false;

let wrapper;

const mountComponent = (anchorInternal) => {
  wrapper = mount(HeaderLink, {
    propsData: {
      anchorId,
      anchorName,
      anchorValue,
      anchorInternal,
      lastPlaceLink,
      anchorAction() {},
    },
    stubs: { 'router-link': '<div data-purpose="router-link"><slot/></div>' },
  });
};

describe('HeaderLink', () => {
  it('will render plain anchor tags for external links', () => {
    mountComponent(false);
    const anchorWrappers = wrapper.findAll('a').wrappers;
    const mappedAttributes = anchorWrappers.map((a) => {
      const attributes = a.attributes();

      return {
        name: a.text().trim(),
        value: attributes.href,
        id: attributes.id,
        target: attributes.target,
        rel: attributes.rel,
      };
    });
    const expectedAttributes = [
      { name: 'Name', value: '/value', id: 'Id', ...additionalInternalAnchorAttrs },
    ];

    expect(mappedAttributes).toEqual(expectedAttributes);
  });

  it('will render router-links for internal links', () => {
    mountComponent(true);
    const routerLinkWrappers = wrapper.findAll('div[data-purpose=router-link]').wrappers;
    const mappedAttributes = routerLinkWrappers.map((r) => {
      const attributes = r.attributes();

      return {
        name: r.text().trim(),
        value: attributes.to,
        id: attributes.id,
      };
    });
    const expectedAttributes = [
      { name: 'Name', value: '/value', id: 'Id' },
    ];

    expect(mappedAttributes).toEqual(expectedAttributes);
  });
});
