/**
 * Initilaize RichTextEditor from React element
 */
import { HtmlEditor, Image, Inject, Link, QuickToolbar, RichTextEditorComponent, Toolbar } from '@syncfusion/ej2-react-richtexteditor';
import * as React from 'react';
import AppConfig from '../core/AppConfig';

interface RichTextProps {
  value: string;
  change?: any;
  attachmentKind?: string;
  targetFolder?: string;
  readOnly?: boolean;
  noButtons?: Array<string>;
  noToolbar?: boolean;
  enabled?: boolean;
  toolbarSettings?: any;
  style?: any;
}

class App extends React.Component<RichTextProps> {
  el: any;

  constructor(props) {
    super(props);
    this.el = React.createRef();
  }
  handleToolbarClick = event => {
    // let el = this.el.current;
    // el.imageModule.uploadObj.uploading = function(args) {
    //   args.customFormData.push('AttachmentKind', 'NCN');
    // };
  };
  render() {
    const { attachmentKind = 'Public', targetFolder = 'Public' } = this.props;
    let saveQueryParams = `AttachmentKind=${attachmentKind}&TargetFolder=${targetFolder}`;
    const { readOnly, noButtons, noToolbar } = this.props;

    let items = [
      'Bold',
      'Italic',
      'Underline',
      '|',
      'Formats',
      'Alignments',
      'OrderedList',
      'UnorderedList',
      '|',
      'CreateLink',
      'Image',
      '|',
      'SourceCode',
      'Undo',
      'Redo'
    ];

    if (noButtons) items = items.filter(i => !noButtons.some(e => e == i));

    return (
      <RichTextEditorComponent
        ref={this.el}
        {...this.props}
        insertImageSettings={{
          saveUrl: AppConfig.BaseURL + 'Attachment.json?' + saveQueryParams,
          path: AppConfig.BaseURL + 'Attachment/download/'
        }}
        toolbarClick={this.handleToolbarClick}
        readonly={readOnly}
        toolbarSettings={{ items, enable: !readOnly && !noToolbar }}
      >
        <Inject services={[Toolbar, Image, Link, HtmlEditor, QuickToolbar]} />
      </RichTextEditorComponent>
    );
  }
}

export default App;
