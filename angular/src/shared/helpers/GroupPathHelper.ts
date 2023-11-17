import { GroupPreviewDto } from '../service-proxies/service-proxies';

export class GroupPathHelper {
  /**
   * Flattens the group hierarchy
   * @param group
   */
  static buildGroupHierarchy(group: GroupPreviewDto): GroupPreviewDto[] {
    const groupHierarchy: GroupPreviewDto[] = [];
    let currentGroup = group;
    while (currentGroup != null) {
      groupHierarchy.push(currentGroup);
      currentGroup = currentGroup.parent;
    }
    groupHierarchy.reverse();
    return groupHierarchy;
  }
}
