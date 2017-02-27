import React from 'react'
import { Button, Form, Loader } from 'semantic-ui-react'

import SchemaForm from 'components/Form'
import { systemFunction as sF } from 'helpers/checkPermissions'
import { wrapper } from 'helpers/locale'
import accountSchema from './schema'

const { func, shape, string } = React.PropTypes

class EditDetails extends React.Component {

  static propTypes = {
    account: shape({
      name: string.isRequired,
      currentPassword: string.isRequired,
      newPassword: string,
      confirmPassword: string,
      phone: string,
      email: string.isRequired,
    }).isRequired,
    fetchAccount: func.isRequired,
    editForm: func.isRequired,
    submitAccount: func.isRequired,
    localize: func.isRequired,
  }

  componentDidMount() {
    this.props.fetchAccount()
  }

  handleEdit = (e, { name, value }) => {
    this.props.editForm({ name, value })
  }

  handleSubmit = (e) => {
    e.preventDefault()
    if (sF('AccountEdit')) {
      this.props.submitAccount(this.props.account)
    }
  }

  renderForm() {
    const {
      account: { name, currentPassword, newPassword, confirmPassword, phone, email },
      localize,
    } = this.props
    return (
      <SchemaForm
        data={this.props.account}
        schema={accountSchema}
        onSubmit={this.handleSubmit}
      >
        <Form.Input
          value={name}
          onChange={this.handleEdit}
          name="name"
          label={localize('Name')}
          placeholder={localize('NameValueRequired')}
          required
        />
        <Form.Input
          value={currentPassword || ''}
          onChange={this.handleEdit}
          name="currentPassword"
          type="password"
          label={localize('CurrentPassword')}
          placeholder={localize('CurrentPassword')}
          required
        />
        <Form.Input
          value={newPassword || ''}
          onChange={this.handleEdit}
          name="newPassword"
          type="password"
          label={localize('NewPassword_LeaveItEmptyIfYouWillNotChangePassword')}
          placeholder={localize('NewPassword')}
        />
        <Form.Input
          value={confirmPassword || ''}
          onChange={this.handleEdit}
          name="confirmPassword"
          type="password"
          label={localize('ConfirmPassword')}
          placeholder={localize('ConfirmPassword')}
          error={newPassword
            && newPassword.length > 0
            && newPassword !== confirmPassword}
        />
        <Form.Input
          value={phone || ''}
          onChange={this.handleEdit}
          name="phone"
          type="tel"
          label={localize('Phone')}
          placeholder={localize('PhoneValueRequired')}
        />
        <Form.Input
          value={email}
          onChange={this.handleEdit}
          name="email"
          type="email"
          label={localize('Email')}
          placeholder={localize('EmailValueRequired')}
          required
        />
        <Button type="submit" primary>{localize('Submit')}</Button>
      </SchemaForm>
    )
  }

  render() {
    return (
      <div>
        <h2>{this.props.localize('EditAccount')}</h2>
        {this.props.account === undefined
          ? <Loader active />
          : this.renderForm()}
      </div>
    )
  }
}

export default wrapper(EditDetails)
