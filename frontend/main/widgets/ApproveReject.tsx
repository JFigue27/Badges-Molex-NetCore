import React from 'react';
import Dialog from './Dialog';
import { Typography, Grid, ButtonBase, Icon } from '@material-ui/core';

interface Props {
  opener: any;
  id?: string;
  message: string;
  onApprove: any;
  onReject: any;
}

class ConfirmWidget extends React.Component<Props> {
  render() {
    const { opener, id = 'approval', message, onApprove, onReject } = this.props;

    return (
      <Dialog opener={opener} id={id} actionsOff>
        {dialog => {
          dialog.onOk = opener.closeDialog.bind(null, id, 'ok');
          //Default onClose is defined in Dialog widget.
          return (
            <>
              <Typography>{message || 'Approval.'}</Typography>
              <Grid container direction='row'>
                <Grid item xs={6}>
                  <ButtonBase style={{ margin: 'auto', display: 'block' }} className='BottomSheetButton' onClick={onApprove}>
                    <Grid container direction='column'>
                      <Grid item>
                        <Icon fontSize='large'>thumb_up</Icon>
                      </Grid>
                      <Grid item>
                        <Typography variant='button'>Approve</Typography>
                      </Grid>
                    </Grid>
                  </ButtonBase>
                </Grid>
                <Grid item xs={6}>
                  <ButtonBase style={{ margin: 'auto', display: 'block' }} className='BottomSheetButton' onClick={onReject}>
                    <Grid container direction='column'>
                      <Grid item>
                        <Icon fontSize='large'>thumb_down</Icon>
                      </Grid>
                      <Grid item>
                        <Typography variant='button'>Reject</Typography>
                      </Grid>
                    </Grid>
                  </ButtonBase>
                </Grid>
              </Grid>
            </>
          );
        }}
      </Dialog>
    );
  }
}

export default ConfirmWidget;
