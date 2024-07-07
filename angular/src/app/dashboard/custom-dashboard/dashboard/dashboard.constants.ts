import {DashboardTileType} from '../../../../shared/service-proxies/service-proxies';
import {NgGridStackOptions} from 'gridstack/dist/angular';

export const CONSTRAINTSBYTYPE: Record<
  DashboardTileType,
  {minW: number; minH: number; maxW: number; maxH: number}
> = {
  [DashboardTileType._0]: {minW: 2, minH: 2, maxW: 6, maxH: 3},
  [DashboardTileType._1]: {minW: 2, minH: 2, maxW: 6, maxH: 8},
  [DashboardTileType._2]: {minW: 2, minH: 2, maxW: 6, maxH: 4},
  [DashboardTileType._3]: {minW: 1, minH: 2, maxW: 12, maxH: 8},
};

export const GRIDOPTIONS: NgGridStackOptions = {
  margin: 5,
  float: true,
  removable: '.trash',
  cellHeight: '3.3rem',
};
