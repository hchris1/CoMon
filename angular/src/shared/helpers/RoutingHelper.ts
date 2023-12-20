export class RoutingHelper {
  /* eslint-disable-next-line @typescript-eslint/no-explicit-any */
  static buildEditModeQueryParams(editMode: boolean): any {
    if (!editMode) return {editMode: null};

    return {editMode: editMode};
  }
}
