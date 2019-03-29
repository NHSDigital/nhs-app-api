import naturalSort from 'javascript-natural-sort';

/**
 * Compares two objects that have a name property - i.e a named item.
 * i.e.
 * <pre>
 * { name: 'some named item' }
 * </pre>
 * @param namedItem is the first operand of the comparator to
 *    be compared against another using JavaScript's local compare
 * @param anotherNamedItem the other operand in the comparison
 * @returns {number} --1 if {namedItem} is less than {anotherNamedItem},
 *    0 if  equivalent and +1 otherwise.
 */
export default (namedItem = {}, anotherNamedItem = {}) =>
  naturalSort((namedItem.name || '').toLocaleLowerCase(),
    (anotherNamedItem.name || '').toLocaleLowerCase());
