import { CoMonTemplatePage } from './app.po';

describe('CoMon App', function() {
  let page: CoMonTemplatePage;

  beforeEach(() => {
    page = new CoMonTemplatePage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
