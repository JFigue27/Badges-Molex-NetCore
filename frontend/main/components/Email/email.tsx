import React from 'react';
import { withRouter } from 'next/router';
import { NoSsr, Typography, Grid } from '@material-ui/core';
import FormContainer, { FormProps } from '../../core/FormContainer';
import { withSnackbar } from 'notistack';

import { Container } from '@material-ui/core';
import Chips from '../../widgets/Chips';
import { InputBase } from '@material-ui/core';
import RichTextEditor from '../../widgets/RichTextEditor';

import EmailService from './email.service';
///start:slot:dependencies<<<
import { TextField } from '@material-ui/core';
///end:slot:dependencies<<<

const service = new EmailService();
const defaultConfig = {
  service
  ///start:slot:config<<<///end:slot:config<<<
};

interface EmailProps extends FormProps {}

class EmailForm extends FormContainer<EmailProps> {
  constructor(props, config) {
    super(props, Object.assign(defaultConfig, config));

    ///start:slot:ctor<<<///end:slot:ctor<<<
  }

  componentDidMount() {
    this.load(this.props.data);
    this.service.GetAccounts().then(accounts => this.setState({ accounts }));
    ///start:slot:load<<<///end:slot:load<<<
  }

  AFTER_LOAD = entity => {
    this.setState({ isDisabled: false });
    ///start:slot:afterLoad<<<///end:slot:afterLoad<<<
  };

  AFTER_CREATE = instance => {
    ///start:slot:afterCreate<<<///end:slot:afterCreate<<<
  };

  AFTER_CREATE_AND_CHECKOUT = entity => {
    ///start:slot:afterCreateCheckout<<<///end:slot:afterCreateCheckout<<<
  };

  AFTER_SAVE = entity => {
    const { dialog } = this.props;
    if (dialog) dialog.close('ok');

    ///start:slot:afterSave<<<///end:slot:afterSave<<<
  };

  BEFORE_CHECKIN = async entity => {
    ///start:slot:beforeCheckin<<<///end:slot:beforeCheckin<<<
    return entity;
  };

  ///start:slot:js<<<///end:slot:js<<<

  render() {
    const { dialog } = this.props;
    if (dialog) dialog.onOk = this.onDialogOk;

    const { isLoading, isDisabled, baseEntity } = this.state;

    const { accounts = [] } = this.state as any;

    ///start:slot:render<<<///end:slot:render<<<

    return (
      <NoSsr>
        {/* <pre>{JSON.stringify(baseEntity, null, 3)}</pre> */}
        <Container style={{ padding: 20 }} maxWidth={false}>
          <Grid container direction='column' item xs={12}>
            <Typography variant='body1' className='body' gutterBottom>
              To
            </Typography>
            <Chips
              placeholder='To'
              options={accounts}
              value={baseEntity.ToList || ''}
              onChange={event => this.handleChipsChange(event, 'ToList')}
              labelProp='UserName'
            />
            <Typography variant='body1' className='body' gutterBottom>
              Cc
            </Typography>
            <Chips
              placeholder='Cc'
              options={accounts}
              value={baseEntity.CcList || ''}
              onChange={event => this.handleChipsChange(event, 'CcList')}
              labelProp='UserName'
            />
            <Typography variant='body1' className='body' gutterBottom>
              Bcc
            </Typography>
            <Chips
              placeholder='Bcc'
              options={accounts}
              value={baseEntity.BccList || ''}
              onChange={event => this.handleChipsChange(event, 'BccList')}
              labelProp='UserName'
            />
            <TextField
              type='text'
              label='Subject'
              value={baseEntity.Subject || ''}
              onChange={event => this.handleInputChange(event, 'Subject')}
              style={{ textAlign: 'left' }}
              margin='dense'
              disabled={isDisabled}
              fullWidth
            />
            <RichTextEditor
              value={baseEntity.Body || ''}
              change={event => this.handleRichText(event, 'Body')}
              enabled={!isDisabled}
              toolbarSettings={{ enable: !isDisabled }}
              style={{ marginTop: 30 }}
            />

            {/* <Attachments
              owner={baseEntity}
              kind='EmailAttachments'
              folderBind='AttachmentsFolder'
              listBind='Attachments'
              readOnly={isDisabled}
              onChange={this.onAttachmentsChange}
            /> */}
            <TextField
              type='text'
              label='Additional Note'
              value={baseEntity.AdditionalNote || ''}
              onChange={event => this.handleInputChange(event, 'AdditionalNote')}
              style={{ textAlign: 'left' }}
              margin='dense'
              disabled={isDisabled}
              fullWidth
            />
          </Grid>
        </Container>
      </NoSsr>
    );
  }
}

export default withSnackbar(withRouter(EmailForm) as any) as React.ComponentClass<EmailProps>;
