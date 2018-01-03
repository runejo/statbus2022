import { bindActionCreators } from 'redux'
import { connect } from 'react-redux'
import { createSelector } from 'reselect'
import { pipe } from 'ramda'

import createSchemaFormHoc from 'components/createSchemaFormHoc'
import FormBody from 'components/StatUnitFormBody'
import createStatUnitSchema from 'helpers/createStatUnitSchema'
import {
  createFieldsMeta,
  createModel,
  createValues,
  updateProperties,
} from 'helpers/modelProperties'
import { getText } from 'helpers/locale'
import { details as actions } from '../actions'

const withSchemaForm = createSchemaFormHoc(props => props.schema, props => props.values)

const withConnect = connect(
  () =>
    createSelector(
      [
        state => state.locale,
        state => state.analysis.details.logEntry.unitType,
        state => state.analysis.details.logEntry.errors,
        state => state.analysis.details.properties,
        state => state.analysis.details.permissions,
      ],
      (locale, type, errors, properties, permissions) => {
        const schema = createStatUnitSchema(type, permissions)
        const updatedProperties = updateProperties(
          schema.cast(createModel(permissions, properties)),
          properties,
        )
        return {
          schema,
          values: createValues(updatedProperties),
          initialErrors: errors,
          permissions,
          updatedProperties,
          fieldsMeta: createFieldsMeta(type, updatedProperties),
          localize: getText(locale),
        }
      },
    ),
  (dispatch, props) =>
    bindActionCreators(
      {
        onSubmit: actions.submitDetails(props.logId, props.queueId),
        onCancel: actions.navigateBack,
      },
      dispatch,
    ),
)

const enhance = pipe(withSchemaForm, withConnect)

export default enhance(FormBody)