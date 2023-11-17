import { AssetDto, Criticality } from '../service-proxies/service-proxies';

export class DynamicStylesHelper {

  static getHistoricBackgroundClass(isLatest: boolean) {
    if (isLatest)
      return '';
    return 'repeating-linear-gradient(45deg, rgba(0, 0, 0, 0.03), rgba(0, 0, 0, 0.03) 10px, rgba(0, 0, 0, 0.07) 10px, rgba(0, 0, 0, 0.07) 20px)';
  }

  static getEmoji(criticality: Criticality) {
    switch (criticality) {
      case 1:
        return '🟢';
      case 3:
        return '🟡';
      case 5:
        return '🔴';
      default:
        return '🤷';
    }
  }

  static getWorstCriticality(asset: AssetDto) {
    if (!asset.packages || asset.packages.length == 0)
      return null;

    const filteredPackages = asset.packages.filter(p => p.lastCriticality != null);

    if (filteredPackages.length === 0)
      return null;

    const worstPackage = filteredPackages
      .reduce((prev, current) => (prev.lastCriticality > current.lastCriticality) ? prev : current);

    return worstPackage?.lastCriticality;
  }

  static getButtonClass(criticality: Criticality) {
    return 'btn-' + this.getClass(criticality);
  }

  // static getIcon(criticality: Criticality) {
  //   switch (criticality) {
  //     case 1:
  //       return 'fa-check';
  //     case 3:
  //       return 'fa-exclamation-triangle';
  //     case 5:
  //       return 'fa-times';
  //     default:
  //       return 'fa-question';
  //   }
  // }

  static getButtonOutlineClass(criticality: Criticality) {
    return 'btn-outline-' + this.getClass(criticality);
  }

  static getBackgroundClass(criticality: Criticality) {
    return 'bg-' + this.getClass(criticality);
  }

  static getBadgeClass(criticality: Criticality) {
    return 'badge-' + this.getClass(criticality);
  }

  static getClass(criticality: Criticality) {
    switch (criticality) {
      case 1:
        return 'success';
      case 3:
        return 'warning';
      case 5:
        return 'danger';
      default:
        return 'secondary';
    }
  }
}
