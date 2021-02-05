import HeaderLinks from '@/components/widgets/HeaderLinks';
import { mount } from '../../helpers';

const anchorLink2 = { name: 'link 2', value: 'appointments', id: 'link-2-link' };
const anchorLink3 = { name: 'link 3', value: 'prescriptions', id: 'link-3-link' };
const additionalInternalAnchorAttrs = { target: '_blank', rel: 'noopener noreferrer' };

const anchorLinks = [
  { name: 'link 1', value: 'https://test.external/link-1', id: 'link-1-link' },
  { ...anchorLink2, internal: true },
  { ...anchorLink3, internal: true },
  { name: 'link 4', value: 'https://test.external/link-4', id: 'link-4-link' },
];

let wrapper;

const mountComponent = () => {
  wrapper = mount(HeaderLinks, {
    propsData: {
      anchorLinks,
    },
    stubs: { 'router-link': '<div data-purpose="router-link"><slot/></div>' },
  });
};

describe('HeaderLinks', () => {
  beforeEach(() => {
    mountComponent();
  });

  it('will render plain anchor tags for external links', () => {
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
      { ...anchorLinks[0], ...additionalInternalAnchorAttrs },
      { ...anchorLinks[3], ...additionalInternalAnchorAttrs },
    ];

    expect(mappedAttributes).toEqual(expectedAttributes);
  });

  it('will render router-links for internal links', () => {
    const routerLinkWrappers = wrapper.findAll('div[data-purpose=router-link]').wrappers;
    const mappedAttributes = routerLinkWrappers.map((r) => {
      const attributes = r.attributes();

      return {
        name: r.text().trim(),
        value: attributes.to,
        id: attributes.id,
      };
    });
    const expectedAttributes = [anchorLink2, anchorLink3];

    expect(mappedAttributes).toEqual(expectedAttributes);
  });
});
