import React from 'react';
import Dialog from './Dialog';
import { Typography } from '@material-ui/core';

interface ConfirmProps {
  opener: any;
  id?: string;
  okLabel?: string;
}

class ConfirmWidget extends React.Component<ConfirmProps> {
  render() {
    const { opener, id = 'confirm', okLabel = 'OK' } = this.props;

    return (
      <Dialog opener={opener} id={id} okLabel={okLabel}>
        {dialog => {
          dialog.onOk = opener.closeDialog.bind(null, id, 'ok');
          //Default onClose is defined in Dialog widget.
          return <Typography>{opener.state[id]}</Typography>;
        }}
      </Dialog>
    );
  }
}

export default ConfirmWidget;
