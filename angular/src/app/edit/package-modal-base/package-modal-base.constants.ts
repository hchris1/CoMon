import {
  HttpPackageBodyEncoding,
  HttpPackageMethod,
  PackageType,
  RtspPackageMethod,
} from '../../../shared/service-proxies/service-proxies';

export const PACKAGETYPES: {
  value: PackageType;
  name: string;
}[] = [
  {
    value: PackageType._0,
    name: 'Edit.PackageTypePing',
  },
  {
    value: PackageType._1,
    name: 'Edit.PackageTypeHttp',
  },
  {
    value: PackageType._2,
    name: 'Edit.PackageTypeRtsp',
  },
  {
    value: PackageType._10,
    name: 'Edit.PackageTypeExternal',
  },
];

export const HTTPMETHODS: {value: HttpPackageMethod; name: string}[] = [
  {
    value: HttpPackageMethod._0,
    name: 'Edit.PackageMethodGet',
  },
  {
    value: HttpPackageMethod._1,
    name: 'Edit.PackageMethodPost',
  },
  {
    value: HttpPackageMethod._2,
    name: 'Edit.PackageMethodPut',
  },
  {
    value: HttpPackageMethod._3,
    name: 'Edit.PackageMethodPatch',
  },
  {
    value: HttpPackageMethod._4,
    name: 'Edit.PackageMethodDelete',
  },
];

export const ENCODINGTYPES: {
  value: HttpPackageBodyEncoding;
  name: string;
}[] = [
  {
    value: HttpPackageBodyEncoding._0,
    name: 'Json',
  },
  {
    value: HttpPackageBodyEncoding._1,
    name: 'Xml',
  },
];

export const RTSPMETHODS: {value: RtspPackageMethod; name: string}[] = [
  {
    value: RtspPackageMethod._0,
    name: 'DESCRIBE',
  },
  {
    value: RtspPackageMethod._1,
    name: 'OPTIONS',
  },
  {
    value: RtspPackageMethod._2,
    name: 'PLAY',
  },
  {
    value: RtspPackageMethod._3,
    name: 'PAUSE',
  },
  {
    value: RtspPackageMethod._4,
    name: 'TEARDOWN',
  },
  {
    value: RtspPackageMethod._5,
    name: 'Setup',
  },
];
