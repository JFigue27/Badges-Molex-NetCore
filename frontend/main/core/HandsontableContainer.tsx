import React from 'react';
import ListContainer, { ListProps } from './ListContainer';

class HandsontableContainer extends ListContainer<ListProps> {
  constructor(props, config) {
    super(props, config);
    this.tableRef = React.createRef();
    this.state.handsonSettings = {
      licenseKey: 'non-commercial-and-evaluation',
      colHeaders: true
    };
  }

  // Hooks:=======================================================================

  render() {
    return <></>;
  }
}

export default HandsontableContainer;
