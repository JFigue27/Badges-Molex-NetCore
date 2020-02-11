import React from 'react';
import Button from '@material-ui/core/Button';
import Dialog from '@material-ui/core/Dialog';
import DialogActions from '@material-ui/core/DialogActions';
import DialogContent from '@material-ui/core/DialogContent';
import DialogTitle from '@material-ui/core/DialogTitle';
import withMobileDialog from '@material-ui/core/withMobileDialog';
import Paper from '@material-ui/core/Paper';
import Draggable from 'react-draggable';

interface Props {
  okLabel?: string;
  title?: string;
  open?: boolean;
  onClose?(): void;
  fullScreen?: boolean;
  actionsOff?: boolean;
  opener?: any;
  id?: string;
  maxWidth?: any;
  dividersOff?: boolean;
}

function DraggableDialog(props) {
  return (
    <Draggable
      cancel={
        '.NoDraggable,.MuiFormControl-root,button,.Person-Chip,.MuiInputBase-input,.filled,.MuiSwitch-root,.MuiList-root,.react-swipeable-view-container,.MuiTable-root,.e-rte-content'
      }
    >
      <Paper {...props} />
    </Draggable>
  );
}

class DialogWidget extends React.Component<Props> {
  static propTypes = {};

  constructor(props) {
    super(props);
    this.close = this.close.bind(this);
  }
  onOk = () => {}; //To be defined on children
  close(...args) {
    const { opener, id } = this ? (this.props as any) : { opener: undefined, id: undefined };
    if (opener) opener.closeDialog(id, ...args);
  }

  render() {
    const { okLabel, title, open, maxWidth = 'sm', fullScreen, actionsOff, children, actions, opener, id, fixed, esc } = this.props as any;
    const { dividersOff } = this.props;
    // this.close = opener ? opener.closeDialog.bind(null, id, 'cancel') : this.close;

    return (
      <Dialog
        fullScreen={fullScreen}
        open={open || (opener && id ? !!opener.state[id] : false)}
        onClose={this.close}
        maxWidth={maxWidth}
        fullWidth={true}
        PaperComponent={fixed || fullScreen ? Paper : DraggableDialog}
        disableEscapeKeyDown={!esc}
      >
        {title && <DialogTitle>{title}</DialogTitle>}
        <DialogContent dividers={!dividersOff} style={{ padding: 10 }}>
          {children(this)}
        </DialogContent>

        {!actionsOff &&
          (actions || (
            <DialogActions>
              <Button onClick={() => this.close('cancel')} color='primary'>
                Close
              </Button>

              {okLabel && (
                <Button
                  onClick={() => {
                    this.onOk();
                  }}
                  color='primary'
                >
                  {okLabel == true ? 'OK' : okLabel}
                </Button>
              )}
            </DialogActions>
          ))}
      </Dialog>
    );
  }
}

export default withMobileDialog()(DialogWidget);
