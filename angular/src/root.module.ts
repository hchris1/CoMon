import {NgModule, APP_INITIALIZER, LOCALE_ID, isDevMode} from '@angular/core';
import {BrowserModule} from '@angular/platform-browser';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {HttpClientModule, HTTP_INTERCEPTORS} from '@angular/common/http';

import {ModalModule} from 'ngx-bootstrap/modal';
import {BsDropdownModule} from 'ngx-bootstrap/dropdown';
import {CollapseModule} from 'ngx-bootstrap/collapse';
import {TabsModule} from 'ngx-bootstrap/tabs';

import {AbpHttpInterceptor} from 'abp-ng2-module';

import {SharedModule} from '@shared/shared.module';
import {ServiceProxyModule} from '@shared/service-proxies/service-proxy.module';
import {RootRoutingModule} from './root-routing.module';
import {AppConsts} from '@shared/AppConsts';
import {API_BASE_URL} from '@shared/service-proxies/service-proxies';

import {RootComponent} from './root.component';
import {AppInitializer} from './app-initializer';
import {ToastrModule} from 'ngx-toastr';
import {ServiceWorkerModule} from '@angular/service-worker';

export function getCurrentLanguage(): string {
  if (abp.localization.currentLanguage.name) {
    return abp.localization.currentLanguage.name;
  }

  // todo: Waiting for https://github.com/angular/angular/issues/31465 to be fixed.
  return 'en';
}

@NgModule({
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    SharedModule.forRoot(),
    ModalModule.forRoot(),
    BsDropdownModule.forRoot(),
    CollapseModule.forRoot(),
    TabsModule.forRoot(),
    ServiceProxyModule,
    RootRoutingModule,
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-right',
      closeButton: true,
      timeOut: 10000,
      extendedTimeOut: 10000,
    }),
    ServiceWorkerModule.register('ngsw-worker.js', {
      enabled: !isDevMode(),
      // Register the ServiceWorker as soon as the application is stable
      // or after 30 seconds (whichever comes first).
      registrationStrategy: 'registerWhenStable:30000',
    }),
  ],
  declarations: [RootComponent],
  providers: [
    {provide: HTTP_INTERCEPTORS, useClass: AbpHttpInterceptor, multi: true},
    {
      provide: APP_INITIALIZER,
      useFactory: (appInitializer: AppInitializer) => appInitializer.init(),
      deps: [AppInitializer],
      multi: true,
    },
    {provide: API_BASE_URL, useFactory: () => AppConsts.remoteServiceBaseUrl},
    {
      provide: LOCALE_ID,
      useFactory: getCurrentLanguage,
    },
  ],
  bootstrap: [RootComponent],
})
export class RootModule {}
