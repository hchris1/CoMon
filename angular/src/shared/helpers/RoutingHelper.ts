export class RoutingHelper {
  static buildEditModeQueryParams(editMode: boolean): any {
    if (!editMode) return { editMode: null };

    return { editMode: editMode };
  }
}
