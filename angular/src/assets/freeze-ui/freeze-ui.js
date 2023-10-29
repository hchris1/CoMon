/* eslint-disable */
'use strict';

// taken from https://raw.githubusercontent.com/alexradulescu/FreezeUI, modified and converted to ES5
(function () {
  const freezeHtml = document.createElement('div');
  freezeHtml.classList.add('freeze-ui');
  const freezedItems = [];

  const getSelector = function getSelector(selector) {
    return selector ? selector : 'body';
  };

  const normalizeFreezeDelay = function normalizeFreezeDelay(delay) {
    return delay ? delay : 250;
  };

  const shouldFreezeItem = function shouldFreezeItem(selector) {
    const itemSelector = getSelector(selector);
    return freezedItems.indexOf(itemSelector) >= 0;
  };

  const addFreezedItem = function addFreezedItem(selector) {
    const itemSelector = getSelector(selector);
    freezedItems.push(itemSelector);
  };

  const removeFreezedItem = function removeFreezedItem(selector) {
    const itemSelector = getSelector(selector);

    for (let i = 0; i < freezedItems.length; i++) {
      if (freezedItems[i] === itemSelector) {
        freezedItems.splice(i, 1);
      }
    }
  };

  window.FreezeUI = function () {
    const options =
      arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : {};
    addFreezedItem(options.selector);
    const delay = normalizeFreezeDelay(options.delay);
    setTimeout(() => {
      if (!shouldFreezeItem(options.selector)) {
        return;
      }

      let parent;

      if (options.element) {
        parent = options.element;
      } else {
        parent = document.querySelector(options.selector) || document.body;
      }

      freezeHtml.setAttribute('data-text', options.text || 'Loading');

      if (document.querySelector(options.selector) || options.element) {
        freezeHtml.style.position = 'absolute';
      }

      parent.appendChild(freezeHtml);
    }, delay);
  };

  window.UnFreezeUI = function () {
    const options =
      arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : {};
    removeFreezedItem(options.selector);
    const delay = normalizeFreezeDelay(options.delay) + 250;
    setTimeout(() => {
      let freezeHtml;

      if (options.element) {
        freezeHtml = options.element.querySelector('.freeze-ui');
      } else {
        freezeHtml = document.querySelector('.freeze-ui');
      }

      if (freezeHtml) {
        freezeHtml.classList.remove('is-unfreezing');

        if (freezeHtml.parentElement) {
          freezeHtml.parentElement.removeChild(freezeHtml);
        }
      }
    }, delay);
  };
})();
