import React from 'react'
import { shape, number, func, string, oneOfType, arrayOf, bool } from 'prop-types'
import { Button, Table, Form, Search, Popup, Message } from 'semantic-ui-react'
import debounce from 'lodash/debounce'

import { DateTimeField, SelectField } from 'components/fields'
import { personTypes, personSex } from 'helpers/enums'
import { internalRequest } from 'helpers/request'
import getUid from 'helpers/getUid'
import config from 'helpers/config'
import { getNewName } from '../../../helpers/locale'

const options = {
  sex: [...personSex],
  types: [...personTypes],
}

class PersonEdit extends React.Component {
  static propTypes = {
    data: shape({
      id: number,
      givenName: string.isRequired,
      personalId: oneOfType([string, number]),
      surname: string.isRequired,
      middleName: string.isRequired,
      birthDate: string,
      sex: oneOfType([string, number]),
      role: oneOfType([string, number]).isRequired,
      countryId: oneOfType([string, number]).isRequired,
      phoneNumber: oneOfType([string, number]),
      phoneNumber1: oneOfType([string, number]),
      address: oneOfType([string, number]),
    }),
    newRowId: number,
    onSave: func.isRequired,
    onCancel: func.isRequired,
    localize: func.isRequired,
    locale: string,
    isAlreadyExist: func,
    countries: arrayOf(shape({})),
    disabled: bool,
  }

  static defaultProps = {
    data: {
      id: -1,
      givenName: '',
      personalId: '',
      surname: '',
      middleName: '',
      birthDate: null,
      sex: '',
      role: '',
      countryId: '',
      phoneNumber: '',
      phoneNumber1: '',
      address: '',
    },
    newRowId: -1,
    countries: [],
    isAlreadyExist: () => false,
    disabled: false,
    locale: '',
  }

  state = {
    data: { ...this.props.data, id: this.props.newRowId },
    isLoading: false,
    touched: false,
    isAlreadyExist: false,
  }

  onFieldChange = (_, { name, value }) => {
    this.setState(s => ({
      data: { ...s.data, [name]: value },
      touched: true,
      isAlreadyExist: this.props.isAlreadyExist({ ...s.data, [name]: value }),
    }))
  }

  onPersonChange = (e, { value }) => {
    this.setState({ controlValue: value, isLoading: true }, () => this.searchData(value))
  }

  searchData = debounce((value) => {
    if (value.length > 0) {
      internalRequest({
        url: '/api/persons/search',
        method: 'get',
        queryParams: { wildcard: value },
        onSuccess: (resp) => {
          this.setState(s => ({
            data: { ...s.data },
            results: resp.map(r => ({
              title: `${r.givenName} ${r.middleName === null ? '' : r.middleName} ${
                r.surname === null ? '' : r.surname
              }`,
              givenName: r.givenName,
              personalId: r.personalId,
              surname: r.surname,
              middleName: r.middleName,
              birthDate: r.birthDate,
              sex: r.sex,
              role: r.role,
              countryId: r.countryId,
              phoneNumber: r.phoneNumber,
              phoneNumber1: r.phoneNumber1,
              address: r.address,
              key: getUid(),
            })),
            isLoading: false,
          }))
        },
        onFail: () => {
          this.setState({
            isLoading: false,
            controlValue: value,
          })
        },
      })
    } else {
      this.setState({ isLoading: false })
    }
  }, 250)

  personSelectHandler = (e, { result }) => {
    this.setState(s => ({
      data: {
        ...s.data,
        id: this.props.newRowId,
        givenName: result.givenName,
        personalId: result.personalId,
        surname: result.surname,
        middleName: result.middleName,
        birthDate: result.birthDate,
        sex: result.sex,
        role: result.role,
        countryId: result.countryId,
        phoneNumber: result.phoneNumber,
        phoneNumber1: result.phoneNumber1,
        address: result.address,
      },
      touched: true,
      isAlreadyExist: this.props.isAlreadyExist({
        ...s.data,
        id: this.props.newRowId,
        givenName: result.givenName,
        personalId: result.personalId,
        surname: result.surname,
        middleName: result.middleName,
        birthDate: result.birthDate,
        sex: result.sex,
        role: result.role,
        countryId: result.countryId,
        phoneNumber: result.phoneNumber,
        phoneNumber1: result.phoneNumber1,
        address: result.address,
      }),
    }))
  }

  saveHandler = () => {
    this.props.onSave(this.state.data)
  }

  render() {
    const { localize, countries, disabled, locale } = this.props
    const { data, isLoading, results, controlValue, touched, isAlreadyExist } = this.state
    const asOption = ([k, v]) => ({ value: k, text: localize(v) })
    const personMandatoryFields = config.mandatoryFields.Person
    return (
      <Table.Row>
        <Table.Cell colSpan={8}>
          <Form as="div">
            <Form.Group widths="equal">
              <SelectField
                name="role"
                label="PersonType"
                lookup={15}
                onChange={this.onFieldChange}
                value={data.role}
                localize={localize}
                locale={locale}
                required={personMandatoryFields.Role}
              />
              <Form.Input
                label={localize('StatUnitFormPersonName')}
                name="givenName"
                value={data.givenName}
                onChange={this.onFieldChange}
                disabled={disabled}
                required={personMandatoryFields.GivenName}
              />
            </Form.Group>
            <Form.Group widths="equal">
              <Popup
                trigger={
                  <Form.Field
                    label={localize('PersonsSearch')}
                    control={Search}
                    loading={isLoading}
                    placeholder={localize('PersonsSearch')}
                    onResultSelect={this.personSelectHandler}
                    onSearchChange={this.onPersonChange}
                    results={results}
                    value={controlValue}
                    showNoResults={false}
                    disabled={disabled}
                    fluid
                  />
                }
                content={localize('PersonSearchPopup')}
                position="top left"
              />
              <Form.Input
                label={localize('Surname')}
                name="surname"
                value={data.surname}
                onChange={this.onFieldChange}
                disabled={disabled}
                required={personMandatoryFields.Surname}
              />
            </Form.Group>
            <Form.Group widths="equal">
              <Form.Input
                label={localize('PersonalId')}
                name="personalId"
                value={data.personalId}
                onChange={this.onFieldChange}
                disabled={disabled}
                required={personMandatoryFields.PersonalId}
              />
              <Form.Input
                label={localize('MiddleName')}
                name="middleName"
                value={data.middleName}
                onChange={this.onFieldChange}
                disabled={disabled}
                required={personMandatoryFields.MiddleName}
              />
            </Form.Group>
            <Form.Group widths="equal">
              <div className="field datepicker">
                <label htmlFor="birthDate">{localize('BirthDate')}</label>
                <DateTimeField
                  name="birthDate"
                  value={data.birthDate}
                  onChange={this.onFieldChange}
                  disabled={disabled}
                  localize={localize}
                  required={personMandatoryFields.BirthDate}
                />
              </div>
              <Form.Select
                name="sex"
                label={localize('Sex')}
                placeholder={localize('Sex')}
                value={data.sex}
                onChange={this.onFieldChange}
                options={options.sex.map(asOption)}
                disabled={disabled}
                required={personMandatoryFields.Sex}
              />
            </Form.Group>
            <Form.Group widths="equal">
              <Form.Select
                label={localize('CountryId')}
                placeholder={localize('CountryId')}
                options={countries}
                value={data.countryId}
                name="countryId"
                key="countryId"
                required={personMandatoryFields.NationalityCode}
                search
                onChange={this.onFieldChange}
                disabled={disabled}
              />
            </Form.Group>
            <Form.Group widths="equal">
              <Form.Input
                label={localize('PhoneNumber')}
                name="phoneNumber"
                value={data.phoneNumber}
                onChange={this.onFieldChange}
                disabled={disabled}
                required={personMandatoryFields.Telephone1}
              />
              <Form.Input
                label={localize('PhoneNumber1')}
                name="phoneNumber1"
                value={data.phoneNumber1}
                onChange={this.onFieldChange}
                disabled={disabled}
                required={personMandatoryFields.Telephone2}
              />
            </Form.Group>
            <Form.Group widths="equal">
              <Form.Input
                label={localize('Address')}
                name="address"
                value={data.address}
                onChange={this.onFieldChange}
                disabled={disabled}
                required={personMandatoryFields.Address}
              />
            </Form.Group>
            <Form.Group widths="equal">
              <div className="field right aligned">
                <label htmlFor="saveBtn">&nbsp;</label>
                <Button.Group>
                  <div data-tooltip={localize('ButtonSave')} data-position="top center">
                    <Button
                      id="saveBtn"
                      icon="check"
                      color="green"
                      onClick={this.saveHandler}
                      disabled={
                        disabled ||
                        (personMandatoryFields.GivenName && !data.givenName) ||
                        (personMandatoryFields.Surname && !data.surname) ||
                        (personMandatoryFields.PersonalId && !data.personalId) ||
                        (personMandatoryFields.MiddleName && !data.middleName) ||
                        (personMandatoryFields.BirthDate && !data.birthDate) ||
                        (personMandatoryFields.Role && !data.role) ||
                        (personMandatoryFields.NationalityCode && !data.countryId) ||
                        (personMandatoryFields.Telephone1 && !data.phoneNumber) ||
                        (personMandatoryFields.Telephone2 && !data.phoneNumber1) ||
                        (personMandatoryFields.Address && !data.address) ||
                        (personMandatoryFields.Sex && !data.sex) ||
                        !touched ||
                        isAlreadyExist
                      }
                    />
                  </div>
                  <div data-tooltip={localize('ButtonCancel')} data-position="top center">
                    <Button
                      icon="cancel"
                      color="red"
                      onClick={this.props.onCancel}
                      disabled={disabled}
                    />
                  </div>
                </Button.Group>
              </div>
              {isAlreadyExist && <Message list={[localize('PersonAlreadyExists')]} compact error />}
            </Form.Group>
          </Form>
        </Table.Cell>
      </Table.Row>
    )
  }
}

export default PersonEdit
